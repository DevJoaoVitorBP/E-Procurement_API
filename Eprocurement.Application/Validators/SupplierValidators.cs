using Eprocurement.Application.Contracts;
using FluentValidation;

namespace Eprocurement.Application.Validators
{
    public class CreateSupplierRequestValidator : AbstractValidator<CreateSupplierRequest>
    {
        public CreateSupplierRequestValidator()
        {
            RuleFor(x => x.CorporateName)
                .NotEmpty().WithMessage("Razão social é obrigatória.")
                .MaximumLength(200);

            RuleFor(x => x.DocumentNumber)
                .NotEmpty().WithMessage("Documento é obrigatório.")
                .MaximumLength(30);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório.")
                .EmailAddress().WithMessage("Email inválido.")
                .MaximumLength(150);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Telefone é obrigatório.")
                .MaximumLength(30);
        }
    }
}