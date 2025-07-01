using System;

namespace PicPayClone.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int PayerId { get; set; }
        public int PayeeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User Payer { get; set; }
        public User Payee { get; set; }
    }
}
