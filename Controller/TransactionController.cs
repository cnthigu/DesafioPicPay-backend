using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PicPayClone.Data;
using PicPayClone.Models;
using PicPayClone.Models;
using System.Text.Json;
using System.Threading.Tasks;
using UserModel = PicPayClone.Models.User;



namespace PicPayClone.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        public TransactionController(AppDbContext context, HttpClient httpClient) 
        {
            _context = context;
            _httpClient = httpClient;
        }


        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDTO dto)
        {
            if (dto == null || dto.Amount <= 0)
                return BadRequest("Dados inválidos.");


            await using IDbContextTransaction transactionScope = await _context.Database.BeginTransactionAsync();

            try
            {
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

                var authResponse = await _httpClient.GetAsync("https://util.devi.tools/api/v2/authorize");

                authResponse.EnsureSuccessStatusCode();

                var content = await authResponse.Content.ReadAsStringAsync();
                var authResult = JsonSerializer.Deserialize<AuthorizationResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (authResult?.Status != "success")
                {
                    return StatusCode(403, "Transação não autorizada pelo serviço externo.");
                }

                payer.Balance -= dto.Amount;
                payee.Balance += dto.Amount;

                var transaction = new Transaction
                {
                    PayerId = payer.Id,
                    PayeeId = payee.Id,
                    Amount = dto.Amount,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Transactions.Add(transaction);

                await _context.SaveChangesAsync();

                try
                {
                    var notificationData = new { email = payee.Email, message = $"Você recebeu uma transferência de R${dto.Amount:F2} de {payer.FullName}." };
                    var notificationContent = new StringContent(JsonSerializer.Serialize(notificationData), System.Text.Encoding.UTF8, "application/json");

                    var notificationResponse = await _httpClient.PostAsync("https://util.devi.tools/api/v1/notify", notificationContent);

                    if (!notificationResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Aviso: Falha ao enviar notificação para {payee.Email}. Status: {notificationResponse.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao chamar o serviço de notificação: {ex.Message}");
                }

                await transactionScope.CommitAsync();

                return Ok(new
                {
                    message = "Transação realizada com sucesso",
                    transactionId = transaction.Id
                });
            }
            catch (HttpRequestException)
            {
                await transactionScope.RollbackAsync();
                return StatusCode(503, "Serviço de autorização indisponível no momento.");
            }
            catch (Exception)
            {
                await transactionScope.RollbackAsync();
                return StatusCode(500, "Ocorreu um erro ao processar a transação. O dinheiro foi devolvido.");
            }
        }
    }
}
