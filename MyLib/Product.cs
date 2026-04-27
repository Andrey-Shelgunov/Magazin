using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public int CategoryId { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }

        public Product(int id, string name, string sku, decimal price, decimal costPrice,
                       int categoryId, int stock, bool isActive = true)
        {
            ProductId = id;
            ProductName = name;
            Sku = sku;
            Price = price;
            CostPrice = costPrice;
            CategoryId = categoryId;
            StockQuantity = stock;
            IsActive = isActive;
        }
    }
}
