namespace PicPayClone.Models
{
    public class CreateTransactionDTO
    {
        public int PayerId { get; set; }
        public int PayeeId { get; set; }
        public decimal Amount { get; set; }
    }
}
