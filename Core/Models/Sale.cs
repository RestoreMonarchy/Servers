using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Sale
    {
        public Sale() { }
        public Sale(short productId, string playerId)
        {
            ProductId = productId;
            PlayerId = playerId;
        }

        public int SaleId { get; set; }
        public short ProductId { get; set; }
        public string PlayerId { get; set; }
        public string TransactionId { get; set; }
        public string TransactionType { get; set; }
        public string PayerEmail { get; set; }
        public string PaymentType { get; set; } 
        public string PaymentStatus { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
