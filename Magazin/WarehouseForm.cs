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
    public partial class WarehouseForm : Form
    {
        private DataGridView dgvProducts;
        private Button btnRefresh;
        private GroupBox grpReceive;
        private TextBox txtSku, txtQuantity;
        private Button btnReceive;
        private Label lblTitle;
        private TextBox txtSearch;
        private Button btnSearch;

        public WarehouseForm()
        {
            InitializeComponents();
            LoadProducts();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(950, 600);
            this.Text = "Складской учет";
            this.Padding = new Padding(10, 10, 10, 10);

            lblTitle = new Label
            {
                Text = "Управление складскими остатками",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(10, 28),
                Size = new Size(400, 30)
            };

            Label lblSearch = new Label { Text = "🔍 Поиск:", Location = new Point(10, 62), Size = new Size(60, 25) };
            txtSearch = new TextBox { Location = new Point(80, 60), Size = new Size(200, 25) };
            btnSearch = new Button { Text = "Найти", Location = new Point(290, 58), Size = new Size(80, 30) };
            btnSearch.Click += BtnSearch_Click;

            dgvProducts = new DataGridView
            {
                Location = new Point(10, 100),
                Size = new Size(580, 430),
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            btnRefresh = new Button
            {
                Text = "🔄 Обновить список",
                Location = new Point(10, 540),
                Size = new Size(140, 35),
                BackColor = Color.LightGray
            };
            btnRefresh.Click += (s, e) => LoadProducts();

            grpReceive = new GroupBox
            {
                Text = "📦 Поступление товара от поставщика",
                Location = new Point(620, 100),
                Size = new Size(300, 220),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            Label lblSku = new Label { Text = "Артикул товара:", Location = new Point(15, 35), Size = new Size(110, 25) };
            txtSku = new TextBox { Location = new Point(140, 33), Size = new Size(140, 25) };

            Label lblQuantity = new Label { Text = "Количество:", Location = new Point(15, 80), Size = new Size(110, 25) };
            txtQuantity = new TextBox { Location = new Point(140, 78), Size = new Size(140, 25) };

            btnReceive = new Button
            {
                Text = "✅ Добавить на склад",
                Location = new Point(75, 140),
                Size = new Size(150, 45),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnReceive.Click += BtnReceive_Click;

            grpReceive.Controls.AddRange(new Control[] { lblSku, txtSku, lblQuantity, txtQuantity, btnReceive });

            Label lblHint = new Label
            {
                Text = "💡 Для добавления товара на склад:\nвведите артикул и количество",
                Location = new Point(620, 340),
                Size = new Size(300, 50),
                ForeColor = Color.Gray,
                Font = new Font("Arial", 9)
            };

            this.Controls.AddRange(new Control[] { lblTitle, lblSearch, txtSearch, btnSearch, dgvProducts, btnRefresh, grpReceive, lblHint });
        }

        private void LoadProducts(string search = "")
        {
            var displayList = new System.Collections.Generic.List<object>();

            foreach (var p in DataService.Instance.Products)
            {
                if (!string.IsNullOrEmpty(search))
                {
                    if (!p.ProductName.ToLower().Contains(search.ToLower()) &&
                        !p.Sku.ToLower().Contains(search.ToLower()))
                        continue;
                }

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
                    Наименование = p.ProductName,
                    Артикул = p.Sku,
                    Цена = $"{p.Price:N2} руб.",
                    Закупка = $"{p.CostPrice:N2} руб.",
                    Остаток = p.StockQuantity,
                    Категория = categoryName,
                    Статус = p.IsActive ? "Активен" : "Неактивен"
                });
            }

            dgvProducts.DataSource = null;
            dgvProducts.DataSource = displayList;

            if (dgvProducts.Columns.Count > 0 && dgvProducts.Columns["ProductId"] != null)
                dgvProducts.Columns["ProductId"].Visible = false;

            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadProducts(txtSearch.Text.Trim());
        }

        private void BtnReceive_Click(object sender, EventArgs e)
        {
            string sku = txtSku.Text.Trim();

            if (string.IsNullOrWhiteSpace(sku))
            {
                MessageBox.Show("Введите артикул товара!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var product = DataService.Instance.GetProductBySku(sku);

            if (product == null)
            {
                MessageBox.Show("Товар с таким артикулом не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            product.StockQuantity += quantity;
            DataService.Instance.UpdateProductStock(product.ProductId, product.StockQuantity);

            MessageBox.Show($"✅ Товар \"{product.ProductName}\" добавлен на склад в количестве {quantity} шт.\nНовый остаток: {product.StockQuantity} шт.",
                "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);

            txtSku.Clear();
            txtQuantity.Clear();
            LoadProducts(txtSearch.Text.Trim());
        }
    }
}