using MyLib;
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

namespace Magazin
{
    public partial class SalesForm : Form
    {
        private DataGridView dgvProducts;
        private ListBox lstCart;
        private TextBox txtSearch;
        private Button btnSearch, btnRemove, btnPay;
        private Label lblTotal, lblTotalValue;
        private ComboBox cmbPayment;

        private List<CartItem> cart = new List<CartItem>();

        public SalesForm()
        {
            InitializeComponents();
            LoadProducts();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(900, 550);
            this.Text = "Продажи - Кассовый модуль";

            Label lblSearch = new Label { Text = "Поиск:", Location = new Point(10, 30), Size = new Size(50, 25) };
            txtSearch = new TextBox { Location = new Point(60, 30), Size = new Size(200, 25) };
            btnSearch = new Button { Text = "Найти", Location = new Point(300,30), Size = new Size(80, 30) };
            btnSearch.Click += BtnSearch_Click;

            dgvProducts = new DataGridView
            {
                Location = new Point(10, 70),
                Size = new Size(580, 300),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false
            };
            dgvProducts.CellDoubleClick += DgvProducts_CellDoubleClick;

            Label lblCart = new Label { Text = "Корзина:", Location = new Point(610, 30), Size = new Size(100, 25), Font = new Font("Arial", 10, FontStyle.Bold) };
            lstCart = new ListBox { Location = new Point(610, 70), Size = new Size(260, 250) };

            btnRemove = new Button { Text = "Удалить", Location = new Point(610, 310), Size = new Size(80, 30) };
            btnRemove.Click += BtnRemove_Click;

            lblTotal = new Label { Text = "ИТОГО:", Location = new Point(610, 340), Size = new Size(80, 30), Font = new Font("Arial", 12, FontStyle.Bold) };
            lblTotalValue = new Label { Text = "0.00 руб.", Location = new Point(700, 340), Size = new Size(150, 30), Font = new Font("Arial", 12, FontStyle.Bold), ForeColor = Color.Green };

            Label lblPayment = new Label { Text = "Способ оплаты:", Location = new Point(610, 380), Size = new Size(120, 25) };
            cmbPayment = new ComboBox { Location = new Point(730, 378), Size = new Size(100, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbPayment.Items.AddRange(new string[] { "Наличные", "Карта" });
            cmbPayment.SelectedIndex = 0;

            btnPay = new Button { Text = "ОПЛАТИТЬ", Location = new Point(670, 420), Size = new Size(150, 50), BackColor = Color.LightGreen, Font = new Font("Arial", 12, FontStyle.Bold) };
            btnPay.Click += BtnPay_Click;

            Label lblHint = new Label
            {
                Text = "Совет: Дважды кликните по товару для добавления",
                Location = new Point(10, 360),
                Size = new Size(350, 40),
                ForeColor = Color.Gray,
                Font = new Font("Arial", 8)
            };

            this.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnSearch, dgvProducts, lblCart, lstCart, btnRemove, lblTotal, lblTotalValue, lblPayment, cmbPayment, btnPay, lblHint });
        }

        private void LoadProducts(string search = "")
        {
            List<Product> products;
            if (string.IsNullOrEmpty(search))
            {
                products = new List<Product>();
                foreach (var p in DataService.Instance.Products)
                {
                    if (p.IsActive && p.StockQuantity > 0)
                    {
                        products.Add(p);
                    }
                }
            }
            else
            {
                products = DataService.Instance.SearchProducts(search);
                List<Product> filtered = new List<Product>();
                foreach (var p in products)
                {
                    if (p.StockQuantity > 0)
                    {
                        filtered.Add(p);
                    }
                }
                products = filtered;
            }

            var displayList = new List<object>();
            foreach (var p in products)
            {
                displayList.Add(new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Sku,
                    p.Price,
                    Остаток = p.StockQuantity
                });
            }

            dgvProducts.DataSource = null;
            dgvProducts.DataSource = displayList;

            if (dgvProducts.Columns.Count > 0)
            {
                if (dgvProducts.Columns["ProductId"] != null)
                    dgvProducts.Columns["ProductId"].Visible = false;
                if (dgvProducts.Columns["ProductName"] != null)
                    dgvProducts.Columns["ProductName"].HeaderText = "Товар";
                if (dgvProducts.Columns["Sku"] != null)
                    dgvProducts.Columns["Sku"].HeaderText = "Артикул";
                if (dgvProducts.Columns["Price"] != null)
                {
                    dgvProducts.Columns["Price"].HeaderText = "Цена, руб.";
                    dgvProducts.Columns["Price"].DefaultCellStyle.Format = "F2";
                }
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadProducts(txtSearch.Text);
        }

        private void DgvProducts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int productId = (int)dgvProducts.Rows[e.RowIndex].Cells["ProductId"].Value;
                Product product = null;
                foreach (var p in DataService.Instance.Products)
                {
                    if (p.ProductId == productId)
                    {
                        product = p;
                        break;
                    }
                }

                if (product != null)
                {
                    AddToCart(product);
                }
            }
        }

        private void AddToCart(Product product)
        {
            CartItem existing = null;
            foreach (var item in cart)
            {
                if (item.Product.ProductId == product.ProductId)
                {
                    existing = item;
                    break;
                }
            }

            if (existing != null)
            {
                if (existing.Quantity + 1 > product.StockQuantity)
                {
                    MessageBox.Show($"Недостаточно товара на складе! Остаток: {product.StockQuantity}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                existing.Quantity++;
            }
            else
            {
                if (product.StockQuantity < 1)
                {
                    MessageBox.Show("Товар отсутствует на складе!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                cart.Add(new CartItem(product, 1));
            }
            UpdateCart();
        }

        private void UpdateCart()
        {
            lstCart.DataSource = null;
            lstCart.DisplayMember = "DisplayText";
            lstCart.DataSource = cart;

            decimal currentTotal = 0;
            foreach (var item in cart)
            {
                currentTotal += item.Total;
            }
            lblTotalValue.Text = $"{currentTotal:F2} руб.";
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (lstCart.SelectedItem != null)
            {
                CartItem item = (CartItem)lstCart.SelectedItem;
                cart.Remove(item);
                UpdateCart();
            }
        }

        private void BtnPay_Click(object sender, EventArgs e)
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal total = 0;
            foreach (var item in cart)
            {
                total += item.Total;
            }

            DialogResult result = MessageBox.Show($"Подтвердите оплату на сумму {total:F2} руб.\nСпособ: {cmbPayment.SelectedItem}",
                "Оформление продажи", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Sale sale = DataService.Instance.CreateSale(Session.CurrentUser.StaffId, cmbPayment.SelectedItem.ToString(), cart);

                PrintReceipt(sale);

                MessageBox.Show($"Продажа оформлена!\nЧек №{sale.SaleId}\nСумма: {sale.TotalAmount:F2} руб.",
                    "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);

                cart.Clear();
                UpdateCart();
                LoadProducts();
            }
        }

        private void PrintReceipt(Sale sale)
        {
            string receipt = "========================================\r\n";
            receipt += "           МАГАЗИН \"ФОКУС\"\r\n";
            receipt += "        ул. Центральная, 10, г. Примеров\r\n";
            receipt += "             ИНН 1234567890\r\n";
            receipt += "========================================\r\n";
            receipt += "Кассовый чек № " + sale.SaleId + "\r\n";
            receipt += "Дата: " + sale.CreatedAt.ToString("dd.MM.yyyy HH:mm") + "\r\n";
            receipt += "Кассир: " + Session.CurrentUser.FullName + "\r\n";
            receipt += "----------------------------------------\r\n";
            receipt += "№  Наименование          Кол.   Цена   Сумма\r\n";
            receipt += "----------------------------------------\r\n";

            int i = 1;
            foreach (var item in sale.Items)
            {
                string line = i.ToString() + "  " + item.ProductName;
                while (line.Length < 20) line += " ";
                line += " " + item.Quantity.ToString().PadLeft(3);
                line += "  " + item.Price.ToString("F2").PadLeft(6);
                line += "  " + item.Total.ToString("F2").PadLeft(7);
                receipt += line + "\r\n";
                i++;
            }

            receipt += "----------------------------------------\r\n";
            receipt += "ИТОГО: " + sale.TotalAmount.ToString("F2").PadLeft(36) + "\r\n";
            receipt += sale.PaymentMethod.PadRight(20) + ": " + sale.TotalAmount.ToString("F2").PadLeft(20) + "\r\n";
            receipt += "----------------------------------------\r\n";
            receipt += "Спасибо за покупку!\r\n";
            receipt += "========================================";

            MessageBox.Show(receipt, "Чек", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}