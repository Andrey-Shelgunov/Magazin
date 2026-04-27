using MyLib.Helpers;
using MyLib.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace Magazin
{
    public partial class LoginForm : Form
    {
        private TextBox txtLogin;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblTitle;
        private Label lblError;
        private CheckBox chkShowPassword;

        public LoginForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Магазин Фокус - Вход в систему";
            this.Size = new Size(400, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblTitle = new Label
            {
                Text = "МАГАЗИН \"ФОКУС\"",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(100, 30),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblLogin = new Label
            {
                Text = "Логин:",
                Location = new Point(60, 90),
                Size = new Size(80, 25),
                Font = new Font("Arial", 10)
            };

            txtLogin = new TextBox
            {
                Location = new Point(150, 88),
                Size = new Size(170, 25),
                Font = new Font("Arial", 10)
            };

            Label lblPass = new Label
            {
                Text = "Пароль:",
                Location = new Point(60, 130),
                Size = new Size(80, 25),
                Font = new Font("Arial", 10)
            };

            txtPassword = new TextBox
            {
                Location = new Point(150, 128),
                Size = new Size(170, 25),
                PasswordChar = '*',
                Font = new Font("Arial", 10)
            };

            chkShowPassword = new CheckBox
            {
                Text = "Показать пароль",
                Location = new Point(150, 160),
                Size = new Size(120, 25)
            };
            chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;

            btnLogin = new Button
            {
                Text = "Войти",
                Location = new Point(150, 200),
                Size = new Size(100, 35),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnLogin.Click += BtnLogin_Click;

            lblError = new Label
            {
                Location = new Point(50, 250),
                Size = new Size(300, 30),
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 9)
            };

            this.Controls.AddRange(new Control[] { lblTitle, lblLogin, txtLogin, lblPass, txtPassword, chkShowPassword, btnLogin, lblError });
        }

        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '*';
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(login))
            {
                lblError.Text = "Введите логин!";
                txtLogin.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                lblError.Text = "Введите пароль!";
                txtPassword.Focus();
                return;
            }

            var user = DataService.Instance.AuthenticateUser(login, password);

            if (user != null)
            {
                Session.CurrentUser = user;
                MainForm mainForm = new MainForm();
                mainForm.Show();
                this.Hide();
            }
            else
            {
                lblError.Text = "Неверный логин или пароль!";
                txtPassword.Clear();
                txtLogin.Focus();
            }
        }
    }
}