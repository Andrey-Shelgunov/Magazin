using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLib;
using MyLib.Services;
using System;
using System.Collections.Generic;

namespace MagazinTest
{
    [TestClass]
    public class TestMagazin
    {
       
        
            private DataService dataService;

            [TestInitialize]
            public void Setup()
            {
            dataService = DataService.Instance;
            }

            // ==================== ТЕСТ 1: Проверка входа ====================
            [TestMethod]
            public void Test1_AuthenticateUser_ValidCredentials_ReturnsStaff()
            {
                
                string login = "ivanova";
                string password = "123";

                
                Staff staff = dataService.AuthenticateUser(login, password);

               
                Assert.IsNotNull(staff, "Пользователь с правильным логином и паролем должен быть найден");
                Assert.AreEqual("Иванова", staff.FirstName, "Имя должно быть Иванова");
                Assert.AreEqual("А.С.", staff.LastName, "Фамилия должна быть А.С.");
            }

            // ==================== ТЕСТ 2: Неверный пароль ====================
            [TestMethod]
            public void Test2_AuthenticateUser_InvalidPassword_ReturnsNull()
            {
                
                string login = "ivanova";
                string password = "wrong_password";

              
                Staff staff = dataService.AuthenticateUser(login, password);

               
                Assert.IsNull(staff, "При неверном пароле пользователь не должен быть найден");
            }

            // ==================== ТЕСТ 3: Получение списка товаров ====================
            [TestMethod]
            public void Test3_GetAllProducts_ReturnsProductList()
            {
               
                List<Product> products = dataService.Products;

                
                Assert.IsNotNull(products, "Список товаров не должен быть null");
                Assert.IsTrue(products.Count > 0, "В базе данных должны быть товары");
            }

            // ==================== ТЕСТ 4: Поиск по артикулу ====================
            [TestMethod]
            public void Test4_GetProductBySku_ExistingSku_ReturnsProduct()
            {
               
                string sku = "12335";

                
                Product product = dataService.GetProductBySku(sku);

                
                Assert.IsNotNull(product, "Товар с артикулом 12335 должен существовать");
                Assert.AreEqual("Смартфон X200", product.ProductName);
            }

            // ==================== ТЕСТ 5: Создание продажи ====================
            [TestMethod]
            public void Test5_CreateSale_ValidItems_ReturnsSaleWithCorrectTotal()
            {
                
                int cashierId = 1;
                string paymentMethod = "Наличные";

                Product product = dataService.GetProductBySku("12335");
                Assert.IsNotNull(product, "Товар должен существовать для теста");

                int oldStock = product.StockQuantity;

                List<CartItem> cart = new List<CartItem>();
                cart.Add(new CartItem(product, 1));

                
                Sale sale = dataService.CreateSale(cashierId, paymentMethod, cart);

                Product updatedProduct = dataService.GetProductBySku("12335");

                
                Assert.IsNotNull(sale, "Продажа должна быть создана");
                Assert.IsTrue(sale.SaleId > 0, "ID продажи должен быть больше 0");
                Assert.AreEqual(product.Price, sale.TotalAmount, "Сумма продажи должна равняться цене товара");
                Assert.AreEqual(oldStock - 1, updatedProduct.StockQuantity, "Остаток товара должен уменьшиться на 1");
            }
        }
    }