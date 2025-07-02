namespace DesafioPicPayBackEnd.Models
{
    public class TransactionResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int? TransactionId { get; set; }
        public int StatusCode { get; set; }

        public static TransactionResult Success(int transactionId, string message = "Transação realizada com sucesso")
        {
            return new TransactionResult
            {
                IsSuccess = true,
                Message = message,
                TransactionId = transactionId,
                StatusCode = 200
            };
        }

        public static TransactionResult Failure(string message, int statusCode)
        {
            return new TransactionResult
            {
                IsSuccess = false,
                Message = message,
                StatusCode = statusCode
            };
        }
    }
}
