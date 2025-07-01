namespace PicPayClone.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public decimal Balance { get; set; }
        public UserType Type { get; set; }

        public enum UserType
        {
            Common = 1,
            Merchant = 2
        }
    }
}
