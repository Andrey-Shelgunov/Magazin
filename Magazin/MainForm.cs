using MyLib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Magazin
{
    public partial class MainForm : Form
    {
        private MenuStrip menuStrip;
        private Panel contentPanel;
        private Label lblWelcome;
        private Button btnSales, btnWarehouse, btnAdmin, btnReports, btnLogout;

        public MainForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Магазин Фокус - Главное меню";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.IsMdiContainer = true;

            menuStrip = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("Файл");
            var exitItem = new ToolStripMenuItem("Выход");
            exitItem.Click += (s, e) => Application.Exit();
            fileMenu.DropDownItems.Add(exitItem);

            var helpMenu = new ToolStripMenuItem("Справка");
            var aboutItem = new ToolStripMenuItem("О программе");
            aboutItem.Click += (s, e) => MessageBox.Show("Магазин Фокус v1.0\nАвтоматизированная система учета", "О программе");
            helpMenu.DropDownItems.Add(aboutItem);

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(helpMenu);
            this.Controls.Add(menuStrip);

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = Color.LightGray
            };

            string welcomeText = "Добро пожаловать,\n";
            if (Session.CurrentUser != null)
            {
                welcomeText += Session.CurrentUser.FullName;
            }
            else
            {
                welcomeText += "Гость";
            }

            lblWelcome = new Label
            {
                Text = welcomeText,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(180, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };
            buttonPanel.Controls.Add(lblWelcome);

            int y = 70;

            if (Session.IsCashier || Session.IsAdmin)
            {
                btnSales = CreateMenuButton("Продажи", y);
                btnSales.Click += (s, e) => OpenForm(new SalesForm());
                buttonPanel.Controls.Add(btnSales);
                y += 50;
            }

            if (Session.IsWarehouse || Session.IsAdmin)
            {
                btnWarehouse = CreateMenuButton("Склад", y);
                btnWarehouse.Click += (s, e) => OpenForm(new WarehouseForm());
                buttonPanel.Controls.Add(btnWarehouse);
                y += 50;
            }

            if (Session.IsAdmin)
            {
                btnAdmin = CreateMenuButton("Администрирование", y);
                btnAdmin.Click += (s, e) => OpenForm(new AdminForm());
                buttonPanel.Controls.Add(btnAdmin);
                y += 50;
            }

            btnReports = CreateMenuButton("Отчеты", y);
            btnReports.Click += (s, e) => OpenForm(new ReportsForm());
            buttonPanel.Controls.Add(btnReports);
            y += 50;

            btnLogout = CreateMenuButton("Выход", y);
            btnLogout.BackColor = Color.LightCoral;
            btnLogout.Click += (s, e) =>
            {
                Session.Clear();
                LoginForm login = new LoginForm();
                login.Show();
                this.Close();
            };
            buttonPanel.Controls.Add(btnLogout);

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            this.Controls.Add(contentPanel);
            this.Controls.Add(buttonPanel);
            this.MainMenuStrip = menuStrip;

            ShowWelcomeMessage();
        }

        private Button CreateMenuButton(string text, int y)
        {
            return new Button
            {
                Text = text,
                Location = new Point(10, y),
                Size = new Size(180, 40),
                Font = new Font("Arial", 11),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };
        }

        private void ShowWelcomeMessage()
        {
            Label welcomeLabel = new Label
            {
                Text = "Добро пожаловать в систему\nавтоматизации магазина \"Фокус\"",
                Font = new Font("Arial", 18),
                AutoSize = true,
                Location = new Point(250, 200)
            };
            contentPanel.Controls.Add(welcomeLabel);
        }

        private void OpenForm(Form form)
        {
            contentPanel.Controls.Clear();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(form);
            form.Show();
        }
    }
}