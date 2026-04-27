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
    public partial class ReportsForm : Form
    {
        private DateTimePicker dtpFrom, dtpTo;
        private Button btnShowReport, btnRefresh, btnPrint;
        private Label lblTotalRevenue, lblTotalRevenueValue;
        private DataGridView dgvTopProducts;
        private ListBox lstCategorySales;

        public ReportsForm()
        {
            InitializeComponents();
            LoadReport();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(900, 600);
            this.Text = "Отчеты и аналитика";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Заголовок
            Label lblTitle = new Label
            {
                Text = "Аналитика продаж",
                Font = new Font("Arial", 18, FontStyle.Bold),
                Location = new Point(10, 20),
                Size = new Size(300, 40),
                ForeColor = Color.DarkBlue
            };

            // Выбор периода
            GroupBox grpPeriod = new GroupBox
            {
                Text = "Выберите период",
                Location = new Point(10, 70),
                Size = new Size(860, 90)
            };

            Label lblFrom = new Label { Text = "С:", Location = new Point(20, 35), Size = new Size(30, 25) };
            dtpFrom = new DateTimePicker { Location = new Point(55, 33), Size = new Size(150, 25), Format = DateTimePickerFormat.Short };
            dtpFrom.Value = DateTime.Now.AddDays(-30);

            Label lblTo = new Label { Text = "По:", Location = new Point(230, 35), Size = new Size(30, 25) };
            dtpTo = new DateTimePicker { Location = new Point(265, 33), Size = new Size(150, 25), Format = DateTimePickerFormat.Short };
            dtpTo.Value = DateTime.Now;

            btnShowReport = new Button
            {
                Text = "Показать отчет",
                Location = new Point(440, 30),
                Size = new Size(150, 35),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnShowReport.Click += BtnShowReport_Click;

            btnRefresh = new Button
            {
                Text = "Обновить",
                Location = new Point(610, 30),
                Size = new Size(130, 35),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.Click += (s, e) => LoadReport();

            btnPrint = new Button
            {
                Text = "Печать",
                Location = new Point(760, 30),
                Size = new Size(100, 35),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnPrint.Click += BtnPrint_Click;

            grpPeriod.Controls.AddRange(new Control[] { lblFrom, dtpFrom, lblTo, dtpTo, btnShowReport, btnRefresh, btnPrint });

            // Общая выручка
            GroupBox grpRevenue = new GroupBox
            {
                Text = "Общая выручка",
                Location = new Point(10, 170),
                Size = new Size(860, 80)
            };

            lblTotalRevenue = new Label
            {
                Text = "Выручка за период:",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(20, 30),
                Size = new Size(180, 30)
            };

            lblTotalRevenueValue = new Label
            {
                Text = "0.00 руб.",
                Font = new Font("Arial", 18, FontStyle.Bold),
                Location = new Point(220, 25),
                Size = new Size(300, 40),
                ForeColor = Color.Green
            };

            grpRevenue.Controls.AddRange(new Control[] { lblTotalRevenue, lblTotalRevenueValue });

            // Топ товаров
            GroupBox grpTop = new GroupBox
            {
                Text = "Топ-5 популярных товаров",
                Location = new Point(10, 260),
                Size = new Size(500, 300)
            };

            dgvTopProducts = new DataGridView
            {
                Location = new Point(10, 25),
                Size = new Size(480, 260),
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            grpTop.Controls.Add(dgvTopProducts);

            // Продажи по категориям
            GroupBox grpCategories = new GroupBox
            {
                Text = "Продажи по категориям",
                Location = new Point(520, 260),
                Size = new Size(350, 300)
            };

            lstCategorySales = new ListBox
            {
                Location = new Point(10, 25),
                Size = new Size(330, 260),
                Font = new Font("Arial", 10)
            };

            grpCategories.Controls.Add(lstCategorySales);

            this.Controls.AddRange(new Control[] { lblTitle, grpPeriod, grpRevenue, grpTop, grpCategories });
        }

        private string GenerateReportText()
        {
            try
            {
                DateTime from = dtpFrom.Value.Date;
                DateTime to = dtpTo.Value.Date.AddDays(1).AddSeconds(-1);

                // Получаем данные
                decimal revenue = DataService.Instance.GetTotalRevenue(from, to);
                var topProducts = DataService.Instance.GetTopProducts(5);
                var categorySales = DataService.Instance.GetSalesByCategory(from, to);

                // Формируем отчет в виде строки
                StringBuilder report = new StringBuilder();
                report.AppendLine("========================================");
                report.AppendLine("         МАГАЗИН \"ФОКУС\"");
                report.AppendLine("         ОТЧЕТ О ПРОДАЖАХ");
                report.AppendLine("========================================");
                report.AppendLine($"Период: {from:dd.MM.yyyy} - {dtpTo.Value:dd.MM.yyyy}");
                report.AppendLine($"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                report.AppendLine("========================================");
                report.AppendLine();

                report.AppendLine($"💰 ОБЩАЯ ВЫРУЧКА: {revenue:N2} руб.");
                report.AppendLine();
                report.AppendLine("----------------------------------------");
                report.AppendLine("🏆 ТОП-5 ПОПУЛЯРНЫХ ТОВАРОВ:");
                report.AppendLine("----------------------------------------");

                if (topProducts.Count == 0)
                {
                    report.AppendLine("  Нет данных о продажах");
                }
                else
                {
                    int rank = 1;
                    report.AppendLine("  Место | Товар                    | Кол-во | Сумма");
                    report.AppendLine("  ------|--------------------------|--------|------------");
                    foreach (var item in topProducts)
                    {
                        string productName = item.Product != null ? item.Product.ProductName : "Неизвестно";
                        if (productName.Length > 24) productName = productName.Substring(0, 24);
                        report.AppendLine($"  {rank,4}  | {productName,-24} | {item.Quantity,5}  | {item.Total,10:N2} руб.");
                        rank++;
                    }
                }

                report.AppendLine();
                report.AppendLine("----------------------------------------");
                report.AppendLine("📁 ПРОДАЖИ ПО КАТЕГОРИЯМ:");
                report.AppendLine("----------------------------------------");

                bool hasData = false;
                foreach (var cat in categorySales)
                {
                    if (cat.Value > 0)
                    {
                        report.AppendLine($"  {cat.Key,-15} → {cat.Value} шт.");
                        hasData = true;
                    }
                }

                if (!hasData)
                {
                    report.AppendLine("  Нет продаж за выбранный период");
                }

                report.AppendLine();
                report.AppendLine("========================================");
                report.AppendLine("           Спасибо за работу!");
                report.AppendLine("========================================");

                return report.ToString();
            }
            catch (Exception ex)
            {
                return "Ошибка формирования отчета: " + ex.Message;
            }
        }

        private void BtnShowReport_Click(object sender, EventArgs e)
        {
            string report = GenerateReportText();
            MessageBox.Show(report, "Отчет о продажах", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                string report = GenerateReportText();

                // Диалог сохранения файла
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Сохранить отчет";
                saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.FileName = $"Отчет_о_продажах_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Сохраняем отчет в файл
                    System.IO.File.WriteAllText(saveFileDialog.FileName, report, Encoding.UTF8);
                    MessageBox.Show($"Отчет успешно сохранен!\n\nПуть: {saveFileDialog.FileName}",
                        "Сохранение отчета", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении отчета: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadReport()
        {
            try
            {
                DateTime from = dtpFrom.Value.Date;
                DateTime to = dtpTo.Value.Date.AddDays(1).AddSeconds(-1);

                // Общая выручка
                decimal revenue = DataService.Instance.GetTotalRevenue(from, to);
                lblTotalRevenueValue.Text = $"{revenue:N2} руб.";

                // Топ товаров
                var topProducts = DataService.Instance.GetTopProducts(5);
                var topList = new System.Collections.Generic.List<object>();
                int rank = 1;
                foreach (var item in topProducts)
                {
                    string productName = item.Product != null ? item.Product.ProductName : "Неизвестно";
                    topList.Add(new
                    {
                        Место = rank++,
                        Товар = productName,
                        Количество = item.Quantity,
                        Сумма = $"{item.Total:N2} руб."
                    });
                }

                dgvTopProducts.DataSource = null;
                dgvTopProducts.DataSource = topList;
                dgvTopProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Продажи по категориям
                var categorySales = DataService.Instance.GetSalesByCategory(from, to);
                lstCategorySales.Items.Clear();
                foreach (var cat in categorySales)
                {
                    if (cat.Value > 0)
                    {
                        lstCategorySales.Items.Add($"{cat.Key,-20} → {cat.Value} шт.");
                    }
                }

                if (lstCategorySales.Items.Count == 0)
                {
                    lstCategorySales.Items.Add("Нет продаж за выбранный период");
                }

                if (revenue == 0)
                {
                    lblTotalRevenueValue.ForeColor = Color.Gray;
                }
                else
                {
                    lblTotalRevenueValue.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}