using Eprocurement.Application.Contracts;
using FluentValidation;

namespace Eprocurement.Application.Validators
{
    public class CreateSupplierRequestValidator : AbstractValidator<CreateSupplierRequest>
    {
        public CreateSupplierRequestValidator()
        {
            RuleFor(x => x.CorporateName)
                .NotEmpty().WithMessage("Corporate name is required.")
                .MaximumLength(200);

            RuleFor(x => x.DocumentNumber)
                .NotEmpty().WithMessage("Document number is required.")
                .MaximumLength(30);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email.")
                .MaximumLength(150);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .MaximumLength(30);
        }
    }
}