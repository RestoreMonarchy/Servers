using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class ProductTransaction
    {
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public string PlayerId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
