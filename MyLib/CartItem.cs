using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public decimal Total
        {
            get { return Product.Price * Quantity; }
        }

        public string DisplayText
        {
            get { return $"{Product.ProductName} x{Quantity} = {Total:F2} руб."; }
        }
    }
}
