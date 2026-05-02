using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.Services
{
    public class DataService
    {
        private static DataService _instance;

        public static DataService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataService();
                }
                return _instance;
            }
        }

        private DatabaseService db;

        private DataService()
        {
            db = DatabaseService.Instance;
        }

        // Свойства для обратной совместимости
        public List<Product> Products
        {
            get { return db.GetAllProducts(); }
        }

        public List<Category> Categories
        {
            get { return db.GetAllCategories(); }
        }

        public List<Staff> Staffs
        {
            get { return db.GetAllStaff(); }
        }

        public List<Sale> Sales
        {
            get { return db.GetSalesByDate(DateTime.MinValue, DateTime.MaxValue); }
        }

        // ДОБАВЛЕНО: свойство для Roles
        public List<Role> Roles
        {
            get { return db.GetAllRoles(); }
        }

        // ДОБАВЛЕНО: свойство для Credentials
        public List<UserCredential> Credentials
        {
            get { return db.GetAllCredentials(); }
        }

        // Методы для работы с товарами
        public List<Product> GetProductsByCategory(int categoryId)
        {
            List<Product> result = new List<Product>();
            foreach (Product p in Products)
            {
                if (p.CategoryId == categoryId && p.IsActive)
                {
                    result.Add(p);
                }
            }
            return result;
        }

        public Product GetProductBySku(string sku)
        {
            return db.GetProductBySku(sku);
        }

        public List<Product> SearchProducts(string searchText)
        {
            return db.SearchProducts(searchText);
        }

        public void UpdateProductStock(int productId, int newQuantity)
        {
            db.UpdateProductStock(productId, newQuantity);
        }

        public void AddProduct(Product product)
        {
            db.AddProduct(product);
        }

        public void UpdateProduct(Product product)
        {
            db.UpdateProduct(product);
        }

        // Методы для работы с продажами
        public Sale CreateSale(int cashierId, string paymentMethod, List<CartItem> items)
        {
            Sale sale = new Sale();
            sale.CashierId = cashierId;
            sale.CreatedAt = DateTime.Now;
            sale.PaymentMethod = paymentMethod;
            sale.Items = new List<SaleItem>();

            decimal total = 0;
            foreach (CartItem cartItem in items)
            {
                SaleItem saleItem = new SaleItem(0, 0, cartItem.Product.ProductId,
                    cartItem.Quantity, cartItem.Product.Price, cartItem.Product.ProductName);
                sale.Items.Add(saleItem);
                total += saleItem.Total;
            }
            sale.TotalAmount = total;

            int saleId = db.CreateSale(sale);
            sale.SaleId = saleId;

            return sale;
        }

        public List<Sale> GetSalesByDate(DateTime? from, DateTime? to)
        {
            DateTime start = from ?? DateTime.MinValue;
            DateTime end = to ?? DateTime.MaxValue;
            return db.GetSalesByDate(start, end);
        }

        public decimal GetTotalRevenue(DateTime? from, DateTime? to)
        {
            DateTime start = from ?? DateTime.MinValue;
            DateTime end = to ?? DateTime.MaxValue;
            return db.GetTotalRevenue(start, end);
        }

        // Методы для аналитики
        public List<ProductSalesInfo> GetTopProducts(int count)
        {
            Dictionary<int, ProductSalesInfo> productStats = new Dictionary<int, ProductSalesInfo>();
            List<Sale> sales = GetSalesByDate(null, null);

            foreach (Sale sale in sales)
            {
                foreach (SaleItem si in sale.Items)
                {
                    if (!productStats.ContainsKey(si.ProductId))
                    {
                        Product product = GetProductById(si.ProductId);
                        productStats[si.ProductId] = new ProductSalesInfo(product, 0, 0);
                    }
                    productStats[si.ProductId].Quantity += si.Quantity;
                    productStats[si.ProductId].Total += si.Total;
                }
            }

            List<ProductSalesInfo> result = new List<ProductSalesInfo>(productStats.Values);
            result.Sort((a, b) => b.Quantity.CompareTo(a.Quantity));

            List<ProductSalesInfo> top = new List<ProductSalesInfo>();
            for (int i = 0; i < count && i < result.Count; i++)
            {
                top.Add(result[i]);
            }
            return top;
        }

        public Dictionary<string, int> GetSalesByCategory(DateTime? from, DateTime? to)
        {
            List<Sale> sales = GetSalesByDate(from, to);
            Dictionary<string, int> categorySales = new Dictionary<string, int>();

            foreach (Category cat in Categories)
            {
                categorySales[cat.CategoryName] = 0;
            }

            foreach (Sale sale in sales)
            {
                foreach (SaleItem item in sale.Items)
                {
                    Product product = GetProductById(item.ProductId);
                    if (product != null)
                    {
                        foreach (Category cat in Categories)
                        {
                            if (cat.CategoryId == product.CategoryId)
                            {
                                categorySales[cat.CategoryName] += item.Quantity;
                                break;
                            }
                        }
                    }
                }
            }

            return categorySales;
        }

        private Product GetProductById(int id)
        {
            foreach (Product p in Products)
            {
                if (p.ProductId == id)
                    return p;
            }
            return null;
        }

        // Аутентификация
        public Staff AuthenticateUser(string login, string password)
        {
            return db.AuthenticateUser(login, password);
        }

        // ДОБАВЛЕНО: метод для смены пароля
        public bool ChangePassword(string login, string oldPassword, string newPassword)
        {
            return db.ChangePassword(login, oldPassword, newPassword);
        }

        // Методы для сотрудников
        public void AddStaff(Staff staff)
        {
            db.AddStaff(staff);
        }

        // ДОБАВЛЕНО: методы для работы с учетными записями
        public void AddCredential(UserCredential credential)
        {
            db.AddCredential(credential);
        }

        public void UpdateCredential(UserCredential credential)
        {
            db.UpdateCredential(credential);
        }

        public void DeleteCredential(int credentialId)
        {
            db.DeleteCredential(credentialId);
        }
        public void DeleteProduct(int productId)
        {
            db.DeleteProduct(productId);
        }
    }
}