    using DesafioPicPayBackEnd.Models;
using DesafioPicPayBackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PicPayClone.Data;
using PicPayClone.Exceptions;
using PicPayClone.Models;
using PicPayClone.Services;
using PicPayClone.Services.External;
using System.Text.Json;
using UserModel = PicPayClone.Models.User;

namespace PicPayClone.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;
        private readonly External.IAuthorizationService _authorizationService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(
            AppDbContext context,
            External.IAuthorizationService authorizationService,
            INotificationService notificationService,
            ILogger<TransactionService> logger)
        {
            _context = context;
            _authorizationService = authorizationService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<TransactionResult> ProcessTransferAsync(CreateTransactionDTO dto)
        {
            await using IDbContextTransaction transactionScope = await _context.Database.BeginTransactionAsync();

            try
            {
                var (payer, payee) = await GetUsersAsync(dto.PayerId, dto.PayeeId);

                var validationResult = ValidateTransfer(payer, payee, dto.Amount);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                var isAuthorized = await _authorizationService.AuthorizeTransactionAsync();
                if (!isAuthorized)
                {
                    return TransactionResult.Failure("Transação não autorizada pelo serviço externo.", 403);
                }

                var transaction = await ProcessTransferInternalAsync(payer, payee, dto.Amount);

                await _context.SaveChangesAsync();
                await transactionScope.CommitAsync();

                _ = Task.Run(async () => await _notificationService.SendNotificationAsync(payee, payer, dto.Amount));

                _logger.LogInformation("Transação {TransactionId} processada com sucesso entre {PayerId} e {PayeeId}",
                    transaction.Id, dto.PayerId, dto.PayeeId);

                return TransactionResult.Success(transaction.Id);
            }
            catch (UserNotFoundException ex)
            {
                await transactionScope.RollbackAsync();
                _logger.LogWarning("Usuário não encontrado: {Message}", ex.Message);
                return TransactionResult.Failure(ex.Message, 404);
            }
            catch (AuthorizationServiceException)
            {
                await transactionScope.RollbackAsync();
                _logger.LogWarning("Serviço de autorização indisponível");
                return TransactionResult.Failure("Serviço de autorização indisponível no momento.", 503);
            }
            catch (Exception ex)
            {
                await transactionScope.RollbackAsync();
                _logger.LogError(ex, "Erro inesperado ao processar transação");
                return TransactionResult.Failure("Ocorreu um erro ao processar a transação. O dinheiro foi devolvido.", 500);
            }
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions
                .Include(t => t.Payer)
                .Include(t => t.Payee)
                .ToListAsync();
        }

        private async Task<(UserModel payer, UserModel payee)> GetUsersAsync(int payerId, int payeeId)
        {
            var payer = await _context.Users.FirstOrDefaultAsync(u => u.Id == payerId);
            var payee = await _context.Users.FirstOrDefaultAsync(u => u.Id == payeeId);

            if (payer == null)
                throw new UserNotFoundException("Pagador não encontrado.");
            if (payee == null)
                throw new UserNotFoundException("Recebedor não encontrado.");

            return (payer, payee);
        }

        private static TransactionResult ValidateTransfer(UserModel payer, UserModel payee, decimal amount)
        {
            if (payer.Type != UserModel.UserType.Common)
                return TransactionResult.Failure("Somente usuários comuns podem realizar pagamentos.", 400);

            if (payer.Balance < amount)
                return TransactionResult.Failure("Saldo insuficiente.", 400);

            return TransactionResult.Success(0);
        }

        private async Task<Transaction> ProcessTransferInternalAsync(UserModel payer, UserModel payee, decimal amount)
        {
            payer.Balance -= amount;
            payee.Balance += amount;

            var transaction = new Transaction
            {
                PayerId = payer.Id,
                PayeeId = payee.Id,
                Amount = amount,
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            return transaction;
        }
    }
}