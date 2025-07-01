using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PicPayClone.Data;
using PicPayClone.Models;
using System.Threading.Tasks;
using PicPayClone.Models;
using UserModel = PicPayClone.Models.User;



namespace PicPayClone.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TransactionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDTO dto)
        {
            if (dto == null || dto.Amount <= 0)
                return BadRequest("Dados inválidos.");

            var payer = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.PayerId);
            var payee = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.PayeeId);

            if (payer == null)
                return NotFound("Pagador não encontrado.");
            if (payee == null)
                return NotFound("Recebedor não encontrado.");

            if (payer.Type != UserModel.UserType.Common)
                return BadRequest("Somente usuários comuns podem realizar pagamentos.");

            if (payer.Balance < dto.Amount)
                return BadRequest("Saldo insuficiente.");

            // atualiza saldos
            payer.Balance -= dto.Amount;
            payee.Balance += dto.Amount;

            // cria a transação
            var transaction = new Transaction
            {
                PayerId = payer.Id,
                PayeeId = payee.Id,
                Amount = dto.Amount,
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Transação realizada com sucesso",
                transactionId = transaction.Id
            });
        }
    }
}
