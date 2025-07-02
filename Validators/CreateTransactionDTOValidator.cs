using FluentValidation;
using PicPayClone.Models;

namespace PicPayClone.Validators
{
    public class CreateTransactionDTOValidator : AbstractValidator<CreateTransactionDTO>
    {
        public CreateTransactionDTOValidator()
        {
            RuleFor(x => x.PayerId)
                .GreaterThan(0)
                .WithMessage("ID do pagador deve ser maior que zero.");

            RuleFor(x => x.PayeeId)
                .GreaterThan(0)
                .WithMessage("ID do recebedor deve ser maior que zero.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Valor da transferência deve ser maior que zero.")
                .LessThanOrEqualTo(1000000)
                .WithMessage("Valor da transferência não pode exceder R$ 1.000.000,00.");

            RuleFor(x => x)
                .Must(x => x.PayerId != x.PayeeId)
                .WithMessage("Pagador e recebedor devem ser diferentes.");
        }
    }
}