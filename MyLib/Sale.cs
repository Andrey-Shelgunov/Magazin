using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
   public class Sale
    {
        public int SaleId { get; set; }
        public int CashierId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public List<SaleItem> Items { get; set; }

        public Sale()
        {
            Items = new List<SaleItem>();
        }

        public Sale(int id, int cashierId, DateTime createdAt, decimal total, string paymentMethod)
        {
            SaleId = id;
            CashierId = cashierId;
            CreatedAt = createdAt;
            TotalAmount = total;
            PaymentMethod = paymentMethod;
            Items = new List<SaleItem>();
        }
    }
}
