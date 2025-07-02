using DesafioPicPayBackEnd.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PicPayClone.Models;
using PicPayClone.Services;

namespace PicPayClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IValidator<CreateTransactionDTO> _validator;

        public TransactionController(
            ITransactionService transactionService,
            IValidator<CreateTransactionDTO> validator)
        {
            _transactionService = transactionService;
            _validator = validator;
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDTO dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await _transactionService.ProcessTransferAsync(dto);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    message = result.Message,
                    transactionId = result.TransactionId
                });
            }

            return StatusCode(result.StatusCode, new { message = result.Message });
        }
    }
}