using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class SaleItem
    {
        public int SaleItemId { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; }

        public decimal Total
        {
            get { return Quantity * Price; }
        }

        public SaleItem(int saleItemId, int saleId, int productId, int quantity, decimal price, string productName)
        {
            SaleItemId = saleItemId;
            SaleId = saleId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
            ProductName = productName;
        }
    }
}
