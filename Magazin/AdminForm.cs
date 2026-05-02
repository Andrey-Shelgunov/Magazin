using MyLib;
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

namespace Magazin
{
    public partial class AdminForm : Form
    {
        private TabControl tabControl;
        private DataGridView dgvProducts;
        private DataGridView dgvStaff;
        private DataGridView dgvCredentials;
        private Button btnAddProduct, btnEditProduct, btnDeleteProduct, btnDeactivateProduct;
        private Button btnAddStaff, btnEditStaff, btnDeactivateStaff;
        private Button btnAddCredential, btnEditCredential, btnDeleteCredential;
        private GroupBox grpProductEdit;
        private TextBox txtProductName, txtSku, txtPrice, txtCostPrice, txtStock;
        private ComboBox cmbCategory;
        private Button btnSaveProduct;
        private int currentEditProductId = -1;

        public AdminForm()
        {
            InitializeComponents();
            LoadProducts();
            LoadStaff();
            LoadCredentials();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(950, 650);
            this.Text = "Администрирование - Панель управления";
            this.StartPosition = FormStartPosition.CenterScreen;

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };

            TabPage productPage = new TabPage("📦 Управление товарами");
            TabPage staffPage = new TabPage("👥 Управление сотрудниками");
            TabPage credentialPage = new TabPage("🔐 Учетные записи");

            // ==================== Управление товарами ====================
            dgvProducts = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(600, 350),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };

            // Кнопки управления товарами (ДОБАВЛЕНА КНОПКА УДАЛЕНИЯ)
            btnAddProduct = new Button { Text = "➕ Добавить", Location = new Point(10, 370), Size = new Size(90, 35), BackColor = Color.LightGreen };
            btnAddProduct.Click += BtnAddProduct_Click;

            btnEditProduct = new Button { Text = "✏️ Редактировать", Location = new Point(110, 370), Size = new Size(100, 35), BackColor = Color.LightYellow };
            btnEditProduct.Click += BtnEditProduct_Click;

            btnDeleteProduct = new Button { Text = "🗑️ Удалить", Location = new Point(220, 370), Size = new Size(90, 35), BackColor = Color.LightCoral };
            btnDeleteProduct.Click += BtnDeleteProduct_Click;

            btnDeactivateProduct = new Button { Text = "❌ Снять с продажи", Location = new Point(320, 370), Size = new Size(120, 35), BackColor = Color.Orange };
            btnDeactivateProduct.Click += BtnDeactivateProduct_Click;

            // Панель редактирования товара
            grpProductEdit = new GroupBox
            {
                Text = "Редактирование товара",
                Location = new Point(630, 10),
                Size = new Size(290, 350),
                Visible = false
            };

            Label lblName = new Label { Text = "Название:", Location = new Point(10, 30), Size = new Size(80, 25) };
            txtProductName = new TextBox { Location = new Point(100, 28), Size = new Size(170, 25) };

            Label lblSku = new Label { Text = "Артикул:", Location = new Point(10, 65), Size = new Size(80, 25) };
            txtSku = new TextBox { Location = new Point(100, 63), Size = new Size(170, 25) };

            Label lblPrice = new Label { Text = "Цена:", Location = new Point(10, 100), Size = new Size(80, 25) };
            txtPrice = new TextBox { Location = new Point(100, 98), Size = new Size(170, 25) };

            Label lblCostPrice = new Label { Text = "Закупка:", Location = new Point(10, 135), Size = new Size(80, 25) };
            txtCostPrice = new TextBox { Location = new Point(100, 133), Size = new Size(170, 25) };

            Label lblStock = new Label { Text = "Остаток:", Location = new Point(10, 170), Size = new Size(80, 25) };
            txtStock = new TextBox { Location = new Point(100, 168), Size = new Size(170, 25) };

            Label lblCategory = new Label { Text = "Категория:", Location = new Point(10, 205), Size = new Size(80, 25) };
            cmbCategory = new ComboBox { Location = new Point(100, 203), Size = new Size(170, 25), DropDownStyle = ComboBoxStyle.DropDownList };

            btnSaveProduct = new Button { Text = "💾 Сохранить", Location = new Point(80, 260), Size = new Size(130, 40), BackColor = Color.LightBlue, Font = new Font("Arial", 10, FontStyle.Bold) };
            btnSaveProduct.Click += BtnSaveProduct_Click;

            Button btnCancelEdit = new Button { Text = "Отмена", Location = new Point(80, 310), Size = new Size(130, 30) };
            btnCancelEdit.Click += (s, e) => { grpProductEdit.Visible = false; };

            grpProductEdit.Controls.AddRange(new Control[] { lblName, txtProductName, lblSku, txtSku, lblPrice, txtPrice, lblCostPrice, txtCostPrice, lblStock, txtStock, lblCategory, cmbCategory, btnSaveProduct, btnCancelEdit });

            productPage.Controls.AddRange(new Control[] { dgvProducts, btnAddProduct, btnEditProduct, btnDeleteProduct, btnDeactivateProduct, grpProductEdit });

            // ==================== Управление сотрудниками ====================
            dgvStaff = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(600, 400),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };

            btnAddStaff = new Button { Text = "➕ Добавить", Location = new Point(10, 420), Size = new Size(100, 35), BackColor = Color.LightGreen };
            btnAddStaff.Click += BtnAddStaff_Click;

            btnEditStaff = new Button { Text = "✏️ Редактировать", Location = new Point(120, 420), Size = new Size(110, 35), BackColor = Color.LightYellow };
            btnEditStaff.Click += BtnEditStaff_Click;

            btnDeactivateStaff = new Button { Text = "👋 Уволить", Location = new Point(240, 420), Size = new Size(100, 35), BackColor = Color.LightCoral };
            btnDeactivateStaff.Click += BtnDeactivateStaff_Click;

            staffPage.Controls.AddRange(new Control[] { dgvStaff, btnAddStaff, btnEditStaff, btnDeactivateStaff });

            // ==================== Управление учетными записями ====================
            dgvCredentials = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(600, 400),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };

            btnAddCredential = new Button { Text = "➕ Добавить", Location = new Point(10, 420), Size = new Size(100, 35), BackColor = Color.LightGreen };
            btnAddCredential.Click += BtnAddCredential_Click;

            btnEditCredential = new Button { Text = "✏️ Изменить пароль", Location = new Point(120, 420), Size = new Size(130, 35), BackColor = Color.LightYellow };
            btnEditCredential.Click += BtnEditCredential_Click;

            btnDeleteCredential = new Button { Text = "🗑️ Удалить", Location = new Point(260, 420), Size = new Size(100, 35), BackColor = Color.LightCoral };
            btnDeleteCredential.Click += BtnDeleteCredential_Click;

            credentialPage.Controls.AddRange(new Control[] { dgvCredentials, btnAddCredential, btnEditCredential, btnDeleteCredential });

            tabControl.TabPages.Add(productPage);
            tabControl.TabPages.Add(staffPage);
            tabControl.TabPages.Add(credentialPage);

            this.Controls.Add(tabControl);

            LoadCategories();
        }

        private void LoadCategories()
        {
            cmbCategory.Items.Clear();
            foreach (var cat in DataService.Instance.Categories)
            {
                cmbCategory.Items.Add(cat);
            }
            if (cmbCategory.Items.Count > 0)
                cmbCategory.SelectedIndex = 0;
        }

        private void LoadProducts()
        {
            var displayList = new System.Collections.Generic.List<object>();
            foreach (var p in DataService.Instance.Products)
            {
                string categoryName = "";
                foreach (var c in DataService.Instance.Categories)
                {
                    if (c.CategoryId == p.CategoryId)
                    {
                        categoryName = c.CategoryName;
                        break;
                    }
                }
                displayList.Add(new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Sku,
                    Цена = p.Price,
                    Закупка = p.CostPrice,
                    Остаток = p.StockQuantity,
                    Категория = categoryName,
                    Статус = p.IsActive ? "Активен" : "Неактивен"
                });
            }
            dgvProducts.DataSource = null;
            dgvProducts.DataSource = displayList;
            if (dgvProducts.Columns["ProductId"] != null)
                dgvProducts.Columns["ProductId"].Visible = false;
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void LoadStaff()
        {
            var displayList = new System.Collections.Generic.List<object>();
            foreach (var s in DataService.Instance.Staffs)
            {
                string roleName = "";
                foreach (var r in DataService.Instance.Roles)
                {
                    if (r.RoleId == s.RoleId)
                    {
                        roleName = r.RoleName;
                        break;
                    }
                }
                displayList.Add(new
                {
                    s.StaffId,
                    Имя = s.FirstName,
                    Фамилия = s.LastName,
                    Должность = roleName,
                    Статус = s.IsActive ? "Работает" : "Уволен"
                });
            }
            dgvStaff.DataSource = null;
            dgvStaff.DataSource = displayList;
            if (dgvStaff.Columns["StaffId"] != null)
                dgvStaff.Columns["StaffId"].Visible = false;
        }

        private void LoadCredentials()
        {
            var displayList = new System.Collections.Generic.List<object>();
            foreach (var cred in DataService.Instance.Credentials)
            {
                string staffName = "";
                foreach (var s in DataService.Instance.Staffs)
                {
                    if (s.StaffId == cred.StaffId)
                    {
                        staffName = s.FullName;
                        break;
                    }
                }
                displayList.Add(new
                {
                    cred.CredentialId,
                    cred.Login,
                    Сотрудник = staffName
                });
            }
            dgvCredentials.DataSource = null;
            dgvCredentials.DataSource = displayList;
            if (dgvCredentials.Columns["CredentialId"] != null)
                dgvCredentials.Columns["CredentialId"].Visible = false;
            dgvCredentials.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            grpProductEdit.Text = "➕ Добавление нового товара";
            txtProductName.Clear();
            txtSku.Clear();
            txtPrice.Clear();
            txtCostPrice.Clear();
            txtStock.Clear();
            txtStock.Text = "0";
            if (cmbCategory.Items.Count > 0) cmbCategory.SelectedIndex = 0;
            grpProductEdit.Visible = true;
            currentEditProductId = -1;
        }

        private void BtnEditProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите товар для редактирования!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            currentEditProductId = (int)dgvProducts.SelectedRows[0].Cells["ProductId"].Value;
            Product product = null;
            foreach (var p in DataService.Instance.Products)
            {
                if (p.ProductId == currentEditProductId)
                {
                    product = p;
                    break;
                }
            }

            if (product != null)
            {
                grpProductEdit.Text = "✏️ Редактирование товара";
                txtProductName.Text = product.ProductName;
                txtSku.Text = product.Sku;
                txtPrice.Text = product.Price.ToString();
                txtCostPrice.Text = product.CostPrice.ToString();
                txtStock.Text = product.StockQuantity.ToString();

                for (int i = 0; i < cmbCategory.Items.Count; i++)
                {
                    Category cat = (Category)cmbCategory.Items[i];
                    if (cat.CategoryId == product.CategoryId)
                    {
                        cmbCategory.SelectedIndex = i;
                        break;
                    }
                }
                grpProductEdit.Visible = true;
            }
        }

        // НОВАЯ КНОПКА: Удаление товара
        private void BtnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите товар для удаления!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int productId = (int)dgvProducts.SelectedRows[0].Cells["ProductId"].Value;
            string productName = dgvProducts.SelectedRows[0].Cells["ProductName"].Value.ToString();

            // Подтверждение удаления
            DialogResult result = MessageBox.Show(
                $"Вы уверены, что хотите полностью удалить товар:\n\"{productName}\"?\n\nВНИМАНИЕ! Это действие нельзя отменить!",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // Находим и удаляем товар
                Product productToRemove = null;
                foreach (var p in DataService.Instance.Products)
                {
                    if (p.ProductId == productId)
                    {
                        productToRemove = p;
                        break;
                    }
                }

                if (productToRemove != null)
                {
                    // Удаляем из списка Products (если DataService поддерживает удаление)
                    DataService.Instance.Products.Remove(productToRemove);

                    // Также нужно удалить из базы данных
                    // Добавьте метод DeleteProduct в DatabaseService и DataService
                    // db.DeleteProduct(productId);

                    LoadProducts();
                    MessageBox.Show($"✅ Товар \"{productName}\" удален!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnSaveProduct_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtProductName.Text))
                {
                    MessageBox.Show("Введите название товара!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtSku.Text))
                {
                    MessageBox.Show("Введите артикул!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string name = txtProductName.Text;
                string sku = txtSku.Text;
                decimal price = decimal.Parse(txtPrice.Text);
                decimal costPrice = decimal.Parse(txtCostPrice.Text);
                int stock = int.Parse(txtStock.Text);
                int categoryId = ((Category)cmbCategory.SelectedItem).CategoryId;

                if (currentEditProductId == -1)
                {
                    Product newProduct = new Product(0, name, sku, price, costPrice, categoryId, stock, true);
                    DataService.Instance.AddProduct(newProduct);
                    MessageBox.Show("✅ Товар успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Product product = new Product(currentEditProductId, name, sku, price, costPrice, categoryId, stock, true);
                    DataService.Instance.UpdateProduct(product);
                    MessageBox.Show("✅ Товар успешно обновлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                grpProductEdit.Visible = false;
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeactivateProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0) return;

            int productId = (int)dgvProducts.SelectedRows[0].Cells["ProductId"].Value;
            string productName = dgvProducts.SelectedRows[0].Cells["ProductName"].Value.ToString();

            DialogResult result = MessageBox.Show($"Снять товар \"{productName}\" с продажи?",
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                foreach (var p in DataService.Instance.Products)
                {
                    if (p.ProductId == productId)
                    {
                        p.IsActive = false;
                        break;
                    }
                }
                LoadProducts();
                MessageBox.Show($"✅ Товар \"{productName}\" снят с продажи!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Остальные методы (для сотрудников и учетных записей) остаются без изменений...
        private void BtnAddStaff_Click(object sender, EventArgs e)
        {
            Form staffForm = new Form();
            staffForm.Text = "➕ Добавление сотрудника";
            staffForm.Size = new Size(350, 280);
            staffForm.StartPosition = FormStartPosition.CenterParent;
            staffForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            staffForm.MaximizeBox = false;

            Label lblFirstName = new Label { Text = "Имя:", Location = new Point(20, 20), Size = new Size(80, 25) };
            TextBox txtFirstName = new TextBox { Location = new Point(110, 18), Size = new Size(200, 25) };

            Label lblLastName = new Label { Text = "Фамилия:", Location = new Point(20, 60), Size = new Size(80, 25) };
            TextBox txtLastName = new TextBox { Location = new Point(110, 58), Size = new Size(200, 25) };

            Label lblRole = new Label { Text = "Должность:", Location = new Point(20, 100), Size = new Size(80, 25) };
            ComboBox cmbRole = new ComboBox { Location = new Point(110, 98), Size = new Size(200, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new string[] { "Администратор", "Кассир", "Кладовщик" });
            cmbRole.SelectedIndex = 1;

            Button btnSave = new Button { Text = "Сохранить", Location = new Point(100, 160), Size = new Size(100, 35), BackColor = Color.LightGreen };
            Button btnCancel = new Button { Text = "Отмена", Location = new Point(210, 160), Size = new Size(100, 35) };

            btnSave.Click += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int roleId = cmbRole.SelectedIndex + 1;
                int newId = 1;
                foreach (var staff in DataService.Instance.Staffs)
                {
                    if (staff.StaffId >= newId) newId = staff.StaffId + 1;
                }

                DataService.Instance.Staffs.Add(new Staff(newId, txtFirstName.Text, txtLastName.Text, roleId, true));
                LoadStaff();
                MessageBox.Show("✅ Сотрудник добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                staffForm.Close();
            };

            btnCancel.Click += (s, ev) => staffForm.Close();

            staffForm.Controls.AddRange(new Control[] { lblFirstName, txtFirstName, lblLastName, txtLastName, lblRole, cmbRole, btnSave, btnCancel });
            staffForm.ShowDialog();
        }

        private void BtnEditStaff_Click(object sender, EventArgs e)
        {
            if (dgvStaff.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите сотрудника для редактирования!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show("Редактирование сотрудника будет доступно в следующей версии", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDeactivateStaff_Click(object sender, EventArgs e)
        {
            if (dgvStaff.SelectedRows.Count == 0) return;

            int staffId = (int)dgvStaff.SelectedRows[0].Cells["StaffId"].Value;
            string staffName = dgvStaff.SelectedRows[0].Cells["Имя"].Value.ToString() + " " + dgvStaff.SelectedRows[0].Cells["Фамилия"].Value.ToString();

            DialogResult result = MessageBox.Show($"Уволить сотрудника {staffName}?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                foreach (var s in DataService.Instance.Staffs)
                {
                    if (s.StaffId == staffId)
                    {
                        s.IsActive = false;
                        break;
                    }
                }
                LoadStaff();
                MessageBox.Show($"✅ Сотрудник {staffName} уволен!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnAddCredential_Click(object sender, EventArgs e)
        {
            Form credForm = new Form();
            credForm.Text = "➕ Добавление учетной записи";
            credForm.Size = new Size(400, 300);
            credForm.StartPosition = FormStartPosition.CenterParent;
            credForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            credForm.MaximizeBox = false;

            Label lblLogin = new Label { Text = "Логин:", Location = new Point(20, 20), Size = new Size(80, 25) };
            TextBox txtLogin = new TextBox { Location = new Point(110, 18), Size = new Size(240, 25) };

            Label lblPassword = new Label { Text = "Пароль:", Location = new Point(20, 60), Size = new Size(80, 25) };
            TextBox txtPassword = new TextBox { Location = new Point(110, 58), Size = new Size(240, 25) };

            Label lblStaff = new Label { Text = "Сотрудник:", Location = new Point(20, 100), Size = new Size(80, 25) };
            ComboBox cmbStaff = new ComboBox { Location = new Point(110, 98), Size = new Size(240, 25), DropDownStyle = ComboBoxStyle.DropDownList };

            foreach (var staff in DataService.Instance.Staffs)
            {
                if (staff.IsActive)
                {
                    cmbStaff.Items.Add(staff);
                }
            }
            if (cmbStaff.Items.Count > 0) cmbStaff.SelectedIndex = 0;

            Button btnSave = new Button { Text = "Сохранить", Location = new Point(100, 170), Size = new Size(100, 35), BackColor = Color.LightGreen };
            Button btnCancel = new Button { Text = "Отмена", Location = new Point(210, 170), Size = new Size(100, 35) };

            btnSave.Click += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(txtLogin.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbStaff.SelectedItem == null)
                {
                    MessageBox.Show("Выберите сотрудника!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Staff selectedStaff = (Staff)cmbStaff.SelectedItem;
                int newId = 1;
                foreach (var cred in DataService.Instance.Credentials)
                {
                    if (cred.CredentialId >= newId) newId = cred.CredentialId + 1;
                }

                DataService.Instance.Credentials.Add(new UserCredential(newId, txtLogin.Text, txtPassword.Text, selectedStaff.StaffId));
                LoadCredentials();
                MessageBox.Show("✅ Учетная запись добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                credForm.Close();
            };

            btnCancel.Click += (s, ev) => credForm.Close();

            credForm.Controls.AddRange(new Control[] { lblLogin, txtLogin, lblPassword, txtPassword, lblStaff, cmbStaff, btnSave, btnCancel });
            credForm.ShowDialog();
        }

        private void BtnEditCredential_Click(object sender, EventArgs e)
        {
            if (dgvCredentials.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите учетную запись!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string login = dgvCredentials.SelectedRows[0].Cells["Login"].Value.ToString();

            Form passForm = new Form();
            passForm.Text = "✏️ Смена пароля";
            passForm.Size = new Size(350, 180);
            passForm.StartPosition = FormStartPosition.CenterParent;
            passForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            passForm.MaximizeBox = false;

            Label lblNewPass = new Label { Text = "Новый пароль:", Location = new Point(20, 30), Size = new Size(100, 25) };
            TextBox txtNewPass = new TextBox { Location = new Point(130, 28), Size = new Size(180, 25) };

            Button btnSave = new Button { Text = "Сохранить", Location = new Point(80, 90), Size = new Size(100, 35), BackColor = Color.LightGreen };
            Button btnCancel = new Button { Text = "Отмена", Location = new Point(190, 90), Size = new Size(100, 35) };

            btnSave.Click += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(txtNewPass.Text))
                {
                    MessageBox.Show("Введите новый пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (var cred in DataService.Instance.Credentials)
                {
                    if (cred.Login == login)
                    {
                        cred.Password = txtNewPass.Text;
                        break;
                    }
                }
                LoadCredentials();
                MessageBox.Show("✅ Пароль изменен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                passForm.Close();
            };

            btnCancel.Click += (s, ev) => passForm.Close();

            passForm.Controls.AddRange(new Control[] { lblNewPass, txtNewPass, btnSave, btnCancel });
            passForm.ShowDialog();
        }

        private void BtnDeleteCredential_Click(object sender, EventArgs e)
        {
            if (dgvCredentials.SelectedRows.Count == 0) return;

            string login = dgvCredentials.SelectedRows[0].Cells["Login"].Value.ToString();

            DialogResult result = MessageBox.Show($"Удалить учетную запись {login}?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                UserCredential toRemove = null;
                foreach (var cred in DataService.Instance.Credentials)
                {
                    if (cred.Login == login)
                    {
                        toRemove = cred;
                        break;
                    }
                }
                if (toRemove != null)
                    DataService.Instance.Credentials.Remove(toRemove);

                LoadCredentials();
                MessageBox.Show("✅ Учетная запись удалена!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}