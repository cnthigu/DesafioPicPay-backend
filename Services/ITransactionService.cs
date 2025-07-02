using DesafioPicPayBackEnd.Models;
using PicPayClone.Models;

namespace DesafioPicPayBackEnd.Services
{
    public interface ITransactionService
    {
        Task<TransactionResult> ProcessTransferAsync(CreateTransactionDTO dto);
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
    }
}
