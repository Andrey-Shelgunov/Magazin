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

        public List<Role> Roles { get; private set; }
        public List<Staff> Staffs { get; private set; }
        public List<Category> Categories { get; private set; }
        public List<Product> Products { get; private set; }
        public List<Sale> Sales { get; private set; }
        public List<SaleItem> SaleItems { get; private set; }
        public List<UserCredential> Credentials { get; private set; }

        private int _nextSaleId = 1;
        private int _nextSaleItemId = 1;

        private DataService()
        {
            InitializeData();
        }

        private void InitializeData()
        {
            Roles = new List<Role>();
            Roles.Add(new Role(1, "Администратор"));
            Roles.Add(new Role(2, "Кассир"));
            Roles.Add(new Role(3, "Кладовщик"));

            Staffs = new List<Staff>();
            Staffs.Add(new Staff(1, "Иванова", "А.С.", 2, true));
            Staffs.Add(new Staff(2, "Петров", "В.И.", 3, true));
            Staffs.Add(new Staff(3, "Сидорова", "М.Н.", 1, true));

            Credentials = new List<UserCredential>();
            Credentials.Add(new UserCredential(1, "ivanova", "123", 1));
            Credentials.Add(new UserCredential(2, "petrov", "123", 2));
            Credentials.Add(new UserCredential(3, "sidorova", "admin", 3));

            Categories = new List<Category>();
            Categories.Add(new Category(1, "Смартфоны", 1));
            Categories.Add(new Category(2, "Аксессуары", 2));
            Categories.Add(new Category(3, "Карты памяти", 3));
            Categories.Add(new Category(4, "Ноутбуки", 4));

            Products = new List<Product>();
            Products.Add(new Product(1, "Смартфон X200", "12335", 25000, 18000, 1, 15, true));
            Products.Add(new Product(2, "Смартфон Y550", "34566", 18000, 12000, 1, 8, true));
            Products.Add(new Product(3, "Чехол для X200", "22456", 1500, 800, 2, 30, true));
            Products.Add(new Product(4, "Карта памяти 128GB", "75432", 2000, 1200, 3, 25, true));
            Products.Add(new Product(5, "Зарядное устройство USB-C", "99881", 1200, 700, 2, 20, true));
            Products.Add(new Product(6, "Наушники Bluetooth", "44321", 3500, 2000, 2, 12, true));
            Products.Add(new Product(7, "Ноутбук WorkBook 15", "11223", 55000, 40000, 4, 5, true));
            Products.Add(new Product(8, "Карта памяти 64GB", "75431", 1200, 700, 3, 18, true));

            Sales = new List<Sale>();
            SaleItems = new List<SaleItem>();

            // ДОБАВЛЯЕМ ТЕСТОВЫЕ ПРОДАЖИ
            AddTestSales();
        }

        private void AddTestSales()
        {
            try
            {
                // Продажа 1 (сегодня)
                List<CartItem> testItems1 = new List<CartItem>();
                testItems1.Add(new CartItem(GetProductById(1), 2));
                testItems1.Add(new CartItem(GetProductById(3), 1));
                CreateSale(1, "Наличные", testItems1);

                // Продажа 2 (вчера)
                List<CartItem> testItems2 = new List<CartItem>();
                testItems2.Add(new CartItem(GetProductById(2), 1));
                testItems2.Add(new CartItem(GetProductById(4), 2));
                Sale sale2 = CreateSale(1, "Карта", testItems2);
                sale2.CreatedAt = DateTime.Now.AddDays(-1);

                // Продажа 3 (3 дня назад)
                List<CartItem> testItems3 = new List<CartItem>();
                testItems3.Add(new CartItem(GetProductById(6), 1));
                testItems3.Add(new CartItem(GetProductById(5), 1));
                Sale sale3 = CreateSale(1, "Наличные", testItems3);
                sale3.CreatedAt = DateTime.Now.AddDays(-3);

                // Продажа 4 (неделю назад)
                List<CartItem> testItems4 = new List<CartItem>();
                testItems4.Add(new CartItem(GetProductById(1), 1));
                testItems4.Add(new CartItem(GetProductById(7), 1));
                Sale sale4 = CreateSale(1, "Карта", testItems4);
                sale4.CreatedAt = DateTime.Now.AddDays(-7);

                // Продажа 5 (2 недели назад)
                List<CartItem> testItems5 = new List<CartItem>();
                testItems5.Add(new CartItem(GetProductById(2), 2));
                testItems5.Add(new CartItem(GetProductById(8), 3));
                Sale sale5 = CreateSale(1, "Наличные", testItems5);
                sale5.CreatedAt = DateTime.Now.AddDays(-14);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Ошибка добавления тестовых продаж: " + ex.Message);
            }
        }

        private Product GetProductById(int id)
        {
            foreach (var p in Products)
            {
                if (p.ProductId == id)
                    return p;
            }
            return null;
        }

        public Staff AuthenticateUser(string login, string password)
        {
            UserCredential credential = null;
            foreach (var cred in Credentials)
            {
                if (cred.Login == login && cred.Password == password)
                {
                    credential = cred;
                    break;
                }
            }

            if (credential == null)
            {
                return null;
            }

            foreach (var staff in Staffs)
            {
                if (staff.StaffId == credential.StaffId && staff.IsActive)
                {
                    return staff;
                }
            }

            return null;
        }

        public bool ChangePassword(string login, string oldPassword, string newPassword)
        {
            foreach (var cred in Credentials)
            {
                if (cred.Login == login && cred.Password == oldPassword)
                {
                    cred.Password = newPassword;
                    return true;
                }
            }
            return false;
        }

        public Sale CreateSale(int cashierId, string paymentMethod, List<CartItem> items)
        {
            Sale sale = new Sale();
            sale.SaleId = _nextSaleId++;
            sale.CashierId = cashierId;
            sale.CreatedAt = DateTime.Now;
            sale.PaymentMethod = paymentMethod;
            sale.Items = new List<SaleItem>();

            decimal total = 0;
            int itemId = _nextSaleItemId;

            foreach (CartItem cartItem in items)
            {
                SaleItem saleItem = new SaleItem(itemId++, sale.SaleId, cartItem.Product.ProductId,
                    cartItem.Quantity, cartItem.Product.Price, cartItem.Product.ProductName);
                sale.Items.Add(saleItem);
                SaleItems.Add(saleItem);
                total += saleItem.Total;

                foreach (Product p in Products)
                {
                    if (p.ProductId == cartItem.Product.ProductId)
                    {
                        p.StockQuantity -= cartItem.Quantity;
                        break;
                    }
                }
            }

            sale.TotalAmount = total;
            Sales.Add(sale);
            _nextSaleItemId = itemId;

            return sale;
        }

        public List<Sale> GetSalesByDate(DateTime? from, DateTime? to)
        {
            List<Sale> result = new List<Sale>();

            foreach (Sale sale in Sales)
            {
                bool include = true;
                if (from.HasValue && sale.CreatedAt.Date < from.Value.Date)
                {
                    include = false;
                }
                if (to.HasValue && sale.CreatedAt.Date > to.Value.Date)
                {
                    include = false;
                }
                if (include)
                {
                    result.Add(sale);
                }
            }

            result.Sort((a, b) => b.CreatedAt.CompareTo(a.CreatedAt));
            return result;
        }

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
            foreach (Product p in Products)
            {
                if (p.Sku == sku && p.IsActive)
                {
                    return p;
                }
            }
            return null;
        }

        public List<Product> SearchProducts(string searchText)
        {
            List<Product> result = new List<Product>();
            string lowerSearch = searchText.ToLower();

            foreach (Product p in Products)
            {
                if (p.IsActive && (p.ProductName.ToLower().Contains(lowerSearch) || p.Sku.ToLower().Contains(searchText.ToLower())))
                {
                    result.Add(p);
                }
            }
            return result;
        }

        public void UpdateProductStock(int productId, int newQuantity)
        {
            foreach (Product p in Products)
            {
                if (p.ProductId == productId)
                {
                    p.StockQuantity = newQuantity;
                    break;
                }
            }
        }

        public void AddProduct(Product product)
        {
            int newId = 1;
            foreach (Product p in Products)
            {
                if (p.ProductId >= newId)
                {
                    newId = p.ProductId + 1;
                }
            }
            product.ProductId = newId;
            Products.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            for (int i = 0; i < Products.Count; i++)
            {
                if (Products[i].ProductId == product.ProductId)
                {
                    Products[i].ProductName = product.ProductName;
                    Products[i].Sku = product.Sku;
                    Products[i].Price = product.Price;
                    Products[i].CostPrice = product.CostPrice;
                    Products[i].CategoryId = product.CategoryId;
                    Products[i].StockQuantity = product.StockQuantity;
                    Products[i].IsActive = product.IsActive;
                    break;
                }
            }
        }

        public decimal GetTotalRevenue(DateTime? from, DateTime? to)
        {
            List<Sale> sales = GetSalesByDate(from, to);
            decimal total = 0;
            foreach (Sale s in sales)
            {
                total += s.TotalAmount;
            }
            return total;
        }

        public List<ProductSalesInfo> GetTopProducts(int count)
        {
            Dictionary<int, ProductSalesInfo> productStats = new Dictionary<int, ProductSalesInfo>();

            foreach (SaleItem si in SaleItems)
            {
                if (!productStats.ContainsKey(si.ProductId))
                {
                    Product product = null;
                    foreach (Product p in Products)
                    {
                        if (p.ProductId == si.ProductId)
                        {
                            product = p;
                            break;
                        }
                    }
                    productStats[si.ProductId] = new ProductSalesInfo(product, 0, 0);
                }

                productStats[si.ProductId].Quantity += si.Quantity;
                productStats[si.ProductId].Total += si.Total;
            }

            List<ProductSalesInfo> result = new List<ProductSalesInfo>();
            foreach (var kvp in productStats)
            {
                result.Add(kvp.Value);
            }

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

            // Собираем ID продаж за период
            HashSet<int> saleIds = new HashSet<int>();
            foreach (Sale s in sales)
            {
                saleIds.Add(s.SaleId);
            }

            // Фильтруем позиции продаж
            List<SaleItem> relevantItems = new List<SaleItem>();
            foreach (SaleItem si in SaleItems)
            {
                if (saleIds.Contains(si.SaleId))
                {
                    relevantItems.Add(si);
                }
            }

            // Считаем продажи по категориям
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (Category cat in Categories)
            {
                // Собираем ID товаров в категории
                HashSet<int> productsInCat = new HashSet<int>();
                foreach (Product p in Products)
                {
                    if (p.CategoryId == cat.CategoryId)
                    {
                        productsInCat.Add(p.ProductId);
                    }
                }

                int count = 0;
                foreach (SaleItem si in relevantItems)
                {
                    if (productsInCat.Contains(si.ProductId))
                    {
                        count += si.Quantity;
                    }
                }
                dict[cat.CategoryName] = count;
            }
            return dict;
        }
    }
}