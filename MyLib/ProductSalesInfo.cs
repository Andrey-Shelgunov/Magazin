using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class ProductSalesInfo
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }

        public ProductSalesInfo(Product product, int quantity, decimal total)
        {
            Product = product;
            Quantity = quantity;
            Total = total;
        }
    }
}
