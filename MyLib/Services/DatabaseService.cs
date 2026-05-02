using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.Services
{
    public class DatabaseService
    {
        private string connectionString;
        private static DatabaseService _instance;

        public static DatabaseService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DatabaseService();
                }
                return _instance;
            }
        }

        private DatabaseService()
        {
            // База данных будет в папке с программой
            string dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shop.db");
            connectionString = $"Data Source={dbPath}";

            // Создаем базу данных, если ее нет
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                // Таблица ролей
                string createRoles = @"
                    CREATE TABLE IF NOT EXISTS Roles (
                        RoleId INTEGER PRIMARY KEY AUTOINCREMENT,
                        RoleName TEXT NOT NULL
                    )";

                // Таблица сотрудников
                string createStaff = @"
                    CREATE TABLE IF NOT EXISTS Staff (
                        StaffId INTEGER PRIMARY KEY AUTOINCREMENT,
                        FirstName TEXT NOT NULL,
                        LastName TEXT NOT NULL,
                        RoleId INTEGER NOT NULL,
                        IsActive INTEGER NOT NULL,
                        FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
                    )";

                // Таблица категорий
                string createCategories = @"
                    CREATE TABLE IF NOT EXISTS Categories (
                        CategoryId INTEGER PRIMARY KEY AUTOINCREMENT,
                        CategoryName TEXT NOT NULL,
                        DisplayOrder INTEGER NOT NULL
                    )";

                // Таблица товаров
                string createProducts = @"
                    CREATE TABLE IF NOT EXISTS Products (
                        ProductId INTEGER PRIMARY KEY AUTOINCREMENT,
                        ProductName TEXT NOT NULL,
                        Sku TEXT UNIQUE NOT NULL,
                        Price REAL NOT NULL,
                        CostPrice REAL NOT NULL,
                        CategoryId INTEGER NOT NULL,
                        StockQuantity INTEGER NOT NULL,
                        IsActive INTEGER NOT NULL,
                        FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
                    )";

                // Таблица учетных записей
                string createCredentials = @"
                    CREATE TABLE IF NOT EXISTS Credentials (
                        CredentialId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Login TEXT UNIQUE NOT NULL,
                        Password TEXT NOT NULL,
                        StaffId INTEGER NOT NULL,
                        FOREIGN KEY (StaffId) REFERENCES Staff(StaffId)
                    )";

                // Таблица продаж
                string createSales = @"
                    CREATE TABLE IF NOT EXISTS Sales (
                        SaleId INTEGER PRIMARY KEY AUTOINCREMENT,
                        CashierId INTEGER NOT NULL,
                        CreatedAt TEXT NOT NULL,
                        TotalAmount REAL NOT NULL,
                        PaymentMethod TEXT NOT NULL,
                        FOREIGN KEY (CashierId) REFERENCES Staff(StaffId)
                    )";

                // Таблица позиций продаж
                string createSaleItems = @"
                    CREATE TABLE IF NOT EXISTS SaleItems (
                        SaleItemId INTEGER PRIMARY KEY AUTOINCREMENT,
                        SaleId INTEGER NOT NULL,
                        ProductId INTEGER NOT NULL,
                        Quantity INTEGER NOT NULL,
                        Price REAL NOT NULL,
                        ProductName TEXT NOT NULL,
                        FOREIGN KEY (SaleId) REFERENCES Sales(SaleId),
                        FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
                    )";

                using (SqliteCommand cmd = new SqliteCommand(createRoles, conn))
                    cmd.ExecuteNonQuery();
                using (SqliteCommand cmd = new SqliteCommand(createStaff, conn))
                    cmd.ExecuteNonQuery();
                using (SqliteCommand cmd = new SqliteCommand(createCategories, conn))
                    cmd.ExecuteNonQuery();
                using (SqliteCommand cmd = new SqliteCommand(createProducts, conn))
                    cmd.ExecuteNonQuery();
                using (SqliteCommand cmd = new SqliteCommand(createCredentials, conn))
                    cmd.ExecuteNonQuery();
                using (SqliteCommand cmd = new SqliteCommand(createSales, conn))
                    cmd.ExecuteNonQuery();
                using (SqliteCommand cmd = new SqliteCommand(createSaleItems, conn))
                    cmd.ExecuteNonQuery();

                // Проверяем, есть ли данные, если нет - добавляем начальные
                CheckAndInsertInitialData(conn);
            }
        }

        private void CheckAndInsertInitialData(SqliteConnection conn)
        {
            // Проверяем роли
            string checkRoles = "SELECT COUNT(*) FROM Roles";
            using (SqliteCommand cmd = new SqliteCommand(checkRoles, conn))
            {
                if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                {
                    // Добавляем роли
                    string insertRoles = @"
                        INSERT INTO Roles (RoleName) VALUES 
                        ('Администратор'),
                        ('Кассир'),
                        ('Кладовщик')";
                    using (SqliteCommand cmd2 = new SqliteCommand(insertRoles, conn))
                        cmd2.ExecuteNonQuery();
                }
            }

            // Проверяем категории
            string checkCategories = "SELECT COUNT(*) FROM Categories";
            using (SqliteCommand cmd = new SqliteCommand(checkCategories, conn))
            {
                if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                {
                    string insertCategories = @"
                        INSERT INTO Categories (CategoryName, DisplayOrder) VALUES 
                        ('Смартфоны', 1),
                        ('Аксессуары', 2),
                        ('Карты памяти', 3),
                        ('Ноутбуки', 4)";
                    using (SqliteCommand cmd2 = new SqliteCommand(insertCategories, conn))
                        cmd2.ExecuteNonQuery();
                }
            }

            // Проверяем сотрудников
            string checkStaff = "SELECT COUNT(*) FROM Staff";
            using (SqliteCommand cmd = new SqliteCommand(checkStaff, conn))
            {
                if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                {
                    string insertStaff = @"
                        INSERT INTO Staff (FirstName, LastName, RoleId, IsActive) VALUES 
                        ('Иванова', 'А.С.', 2, 1),
                        ('Петров', 'В.И.', 3, 1),
                        ('Сидорова', 'М.Н.', 1, 1)";
                    using (SqliteCommand cmd2 = new SqliteCommand(insertStaff, conn))
                        cmd2.ExecuteNonQuery();
                }
            }

            // Проверяем учетные записи
            string checkCredentials = "SELECT COUNT(*) FROM Credentials";
            using (SqliteCommand cmd = new SqliteCommand(checkCredentials, conn))
            {
                if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                {
                    string insertCredentials = @"
                        INSERT INTO Credentials (Login, Password, StaffId) VALUES 
                        ('ivanova', '123', 1),
                        ('petrov', '123', 2),
                        ('sidorova', 'admin', 3)";
                    using (SqliteCommand cmd2 = new SqliteCommand(insertCredentials, conn))
                        cmd2.ExecuteNonQuery();
                }
            }

            // Проверяем товары
            string checkProducts = "SELECT COUNT(*) FROM Products";
            using (SqliteCommand cmd = new SqliteCommand(checkProducts, conn))
            {
                if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                {
                    string insertProducts = @"
                        INSERT INTO Products (ProductName, Sku, Price, CostPrice, CategoryId, StockQuantity, IsActive) VALUES 
                        ('Смартфон X200', '12335', 25000, 18000, 1, 15, 1),
                        ('Смартфон Y550', '34566', 18000, 12000, 1, 8, 1),
                        ('Чехол для X200', '22456', 1500, 800, 2, 30, 1),
                        ('Карта памяти 128GB', '75432', 2000, 1200, 3, 25, 1),
                        ('Зарядное устройство USB-C', '99881', 1200, 700, 2, 20, 1),
                        ('Наушники Bluetooth', '44321', 3500, 2000, 2, 12, 1),
                        ('Ноутбук WorkBook 15', '11223', 55000, 40000, 4, 5, 1),
                        ('Карта памяти 64GB', '75431', 1200, 700, 3, 18, 1)";
                    using (SqliteCommand cmd2 = new SqliteCommand(insertProducts, conn))
                        cmd2.ExecuteNonQuery();
                }
            }
        }

        // ============= МЕТОДЫ ДЛЯ РАБОТЫ С ТОВАРАМИ =============

        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Products";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product p = new Product(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetDecimal(3),
                            reader.GetDecimal(4),
                            reader.GetInt32(5),
                            reader.GetInt32(6),
                            reader.GetInt32(7) == 1
                        );
                        products.Add(p);
                    }
                }
            }
            return products;
        }

        public Product GetProductBySku(string sku)
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Products WHERE Sku = @sku";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@sku", sku);
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Product(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetDecimal(3),
                                reader.GetDecimal(4),
                                reader.GetInt32(5),
                                reader.GetInt32(6),
                                reader.GetInt32(7) == 1
                            );
                        }
                    }
                }
            }
            return null;
        }

        public List<Product> SearchProducts(string searchText)
        {
            List<Product> products = new List<Product>();
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT * FROM Products 
                              WHERE IsActive = 1 
                              AND (ProductName LIKE @search OR Sku LIKE @search)";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@search", $"%{searchText}%");
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product p = new Product(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetDecimal(3),
                                reader.GetDecimal(4),
                                reader.GetInt32(5),
                                reader.GetInt32(6),
                                reader.GetInt32(7) == 1
                            );
                            products.Add(p);
                        }
                    }
                }
            }
            return products;
        }

        public void AddProduct(Product product)
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Products 
                              (ProductName, Sku, Price, CostPrice, CategoryId, StockQuantity, IsActive)
                              VALUES (@name, @sku, @price, @costPrice, @categoryId, @stock, @isActive)";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", product.ProductName);
                    cmd.Parameters.AddWithValue("@sku", product.Sku);
                    cmd.Parameters.AddWithValue("@price", product.Price);
                    cmd.Parameters.AddWithValue("@costPrice", product.CostPrice);
                    cmd.Parameters.AddWithValue("@categoryId", product.CategoryId);
                    cmd.Parameters.AddWithValue("@stock", product.StockQuantity);
                    cmd.Parameters.AddWithValue("@isActive", product.IsActive ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateProduct(Product product)
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = @"UPDATE Products SET 
                              ProductName = @name, 
                              Sku = @sku, 
                              Price = @price, 
                              CostPrice = @costPrice, 
                              CategoryId = @categoryId, 
                              StockQuantity = @stock, 
                              IsActive = @isActive
                              WHERE ProductId = @id";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", product.ProductId);
                    cmd.Parameters.AddWithValue("@name", product.ProductName);
                    cmd.Parameters.AddWithValue("@sku", product.Sku);
                    cmd.Parameters.AddWithValue("@price", product.Price);
                    cmd.Parameters.AddWithValue("@costPrice", product.CostPrice);
                    cmd.Parameters.AddWithValue("@categoryId", product.CategoryId);
                    cmd.Parameters.AddWithValue("@stock", product.StockQuantity);
                    cmd.Parameters.AddWithValue("@isActive", product.IsActive ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateProductStock(int productId, int newQuantity)
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Products SET StockQuantity = @stock WHERE ProductId = @id";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", productId);
                    cmd.Parameters.AddWithValue("@stock", newQuantity);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ============= МЕТОДЫ ДЛЯ РАБОТЫ С КАТЕГОРИЯМИ =============

        public List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Categories ORDER BY DisplayOrder";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Category c = new Category(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetInt32(2)
                        );
                        categories.Add(c);
                    }
                }
            }
            return categories;
        }

        // ============= МЕТОДЫ ДЛЯ РАБОТЫ СОТРУДНИКАМИ =============

        public List<Staff> GetAllStaff()
        {
            List<Staff> staff = new List<Staff>();
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Staff";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Staff s = new Staff(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetInt32(3),
                            reader.GetInt32(4) == 1
                        );
                        staff.Add(s);
                    }
                }
            }
            return staff;
        }

        public void AddStaff(Staff staff)
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Staff (FirstName, LastName, RoleId, IsActive) 
                              VALUES (@firstName, @lastName, @roleId, @isActive)";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@firstName", staff.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", staff.LastName);
                    cmd.Parameters.AddWithValue("@roleId", staff.RoleId);
                    cmd.Parameters.AddWithValue("@isActive", staff.IsActive ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ============= МЕТОДЫ ДЛЯ АВТЕНТИФИКАЦИИ =============

        public Staff AuthenticateUser(string login, string password)
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT s.* FROM Staff s
                              INNER JOIN Credentials c ON s.StaffId = c.StaffId
                              WHERE c.Login = @login AND c.Password = @password AND s.IsActive = 1";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", password);
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Staff(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetInt32(3),
                                reader.GetInt32(4) == 1
                            );
                        }
                    }
                }
            }
            return null;
        }

        // ============= МЕТОДЫ ДЛЯ ПРОДАЖ =============

        public int CreateSale(Sale sale)
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                // Вставляем продажу
                string sql = @"INSERT INTO Sales (CashierId, CreatedAt, TotalAmount, PaymentMethod)
                              VALUES (@cashierId, @createdAt, @totalAmount, @paymentMethod);
                              SELECT last_insert_rowid();";

                int saleId;
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cashierId", sale.CashierId);
                    cmd.Parameters.AddWithValue("@createdAt", sale.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@totalAmount", sale.TotalAmount);
                    cmd.Parameters.AddWithValue("@paymentMethod", sale.PaymentMethod);
                    saleId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Вставляем позиции продажи
                foreach (var item in sale.Items)
                {
                    string sqlItem = @"INSERT INTO SaleItems (SaleId, ProductId, Quantity, Price, ProductName)
                                     VALUES (@saleId, @productId, @quantity, @price, @productName)";
                    using (SqliteCommand cmd = new  SqliteCommand(sqlItem, conn))
                    {
                        cmd.Parameters.AddWithValue("@saleId", saleId);
                        cmd.Parameters.AddWithValue("@productId", item.ProductId);
                        cmd.Parameters.AddWithValue("@quantity", item.Quantity);
                        cmd.Parameters.AddWithValue("@price", item.Price);
                        cmd.Parameters.AddWithValue("@productName", item.ProductName);
                        cmd.ExecuteNonQuery();
                    }

                    // Уменьшаем остаток товара
                    UpdateProductStock(item.ProductId,
                        GetProductById(item.ProductId).StockQuantity - item.Quantity);
                }

                return saleId;
            }
        }

        private Product GetProductById(int id)
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Products WHERE ProductId = @id";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Product(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetDecimal(3),
                                reader.GetDecimal(4),
                                reader.GetInt32(5),
                                reader.GetInt32(6),
                                reader.GetInt32(7) == 1
                            );
                        }
                    }
                }
            }
            return null;
        }

        public List<Sale> GetSalesByDate(DateTime from, DateTime to)
        {
            List<Sale> sales = new List<Sale>();
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT * FROM Sales 
                              WHERE CreatedAt >= @from AND CreatedAt <= @to 
                              ORDER BY CreatedAt DESC";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@from", from.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@to", to.ToString("yyyy-MM-dd HH:mm:ss"));
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Sale sale = new Sale
                            {
                                SaleId = reader.GetInt32(0),
                                CashierId = reader.GetInt32(1),
                                CreatedAt = DateTime.Parse(reader.GetString(2)),
                                TotalAmount = reader.GetDecimal(3),
                                PaymentMethod = reader.GetString(4),
                                Items = GetSaleItems(reader.GetInt32(0))
                            };
                            sales.Add(sale);
                        }
                    }
                }
            }
            return sales;
        }

        private List<SaleItem> GetSaleItems(int saleId)
        {
            List<SaleItem> items = new List<SaleItem>();
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM SaleItems WHERE SaleId = @saleId";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@saleId", saleId);
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SaleItem item = new SaleItem(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetInt32(2),
                                reader.GetInt32(3),
                                reader.GetDecimal(4),
                                reader.GetString(5)
                            );
                            items.Add(item);
                        }
                    }
                }
            }
            return items;
        }

        public decimal GetTotalRevenue(DateTime from, DateTime to)
        {
            decimal total = 0;
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT SUM(TotalAmount) FROM Sales WHERE CreatedAt >= @from AND CreatedAt <= @to";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@from", from.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@to", to.ToString("yyyy-MM-dd HH:mm:ss"));
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                        total = Convert.ToDecimal(result);
                }
            }
            return total;
        }
        // ============= МЕТОДЫ ДЛЯ РАБОТЫ С РОЛЯМИ =============

        public List<Role> GetAllRoles()
        {
            List<Role> roles = new List<Role>();
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Roles";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Role r = new Role(
                            reader.GetInt32(0),
                            reader.GetString(1)
                        );
                        roles.Add(r);
                    }
                }
            }
            return roles;
        }

        // ============= МЕТОДЫ ДЛЯ РАБОТЫ С УЧЕТНЫМИ ЗАПИСЯМИ =============

        public List<UserCredential> GetAllCredentials()
        {
            List<UserCredential> credentials = new List<UserCredential>();
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Credentials";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UserCredential c = new UserCredential(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetInt32(3)
                        );
                        credentials.Add(c);
                    }
                }
            }
            return credentials;
        }

        public void AddCredential(UserCredential credential)
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Credentials (Login, Password, StaffId) 
                      VALUES (@login, @password, @staffId)";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@login", credential.Login);
                    cmd.Parameters.AddWithValue("@password", credential.Password);
                    cmd.Parameters.AddWithValue("@staffId", credential.StaffId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateCredential(UserCredential credential)
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = @"UPDATE Credentials SET 
                      Login = @login, 
                      Password = @password, 
                      StaffId = @staffId
                      WHERE CredentialId = @id";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", credential.CredentialId);
                    cmd.Parameters.AddWithValue("@login", credential.Login);
                    cmd.Parameters.AddWithValue("@password", credential.Password);
                    cmd.Parameters.AddWithValue("@staffId", credential.StaffId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCredential(int credentialId)
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Credentials WHERE CredentialId = @id";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", credentialId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool ChangePassword(string login, string oldPassword, string newPassword)
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Credentials SET Password = @newPassword WHERE Login = @login AND Password = @oldPassword";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@oldPassword", oldPassword);
                    cmd.Parameters.AddWithValue("@newPassword", newPassword);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        public void DeleteProduct(int productId)
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                // Сначала удаляем связанные позиции в SaleItems (если есть)
                string deleteSaleItems = "DELETE FROM SaleItems WHERE ProductId = @id";
                using (SqliteCommand cmd = new SqliteCommand(deleteSaleItems, conn))
                {
                    cmd.Parameters.AddWithValue("@id", productId);
                    cmd.ExecuteNonQuery();
                }

                // Затем удаляем сам товар
                string deleteProduct = "DELETE FROM Products WHERE ProductId = @id";
                using (SqliteCommand cmd = new SqliteCommand(deleteProduct, conn))
                {
                    cmd.Parameters.AddWithValue("@id", productId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
