using Eprocurement.Application.Contracts;
using FluentValidation;

namespace Eprocurement.Application.Validators
{
    public class CreatePurchaseOrderRequestValidator : AbstractValidator<CreatePurchaseOrderRequest>
    {
        public CreatePurchaseOrderRequestValidator()
        {
            RuleFor(x => x.PurchaseRequestId)
                .NotEmpty().WithMessage("Invalid purchase request.");

            RuleFor(x => x.SupplierId)
                .NotEmpty().WithMessage("Invalid supplier.");

            RuleFor(x => x.CreatedByUserId)
                .NotEmpty().WithMessage("Invalid creator user.");
        }
    }

    public class PurchaseOrderActionRequestValidator : AbstractValidator<PurchaseOrderActionRequest>
    {
        public PurchaseOrderActionRequestValidator()
        {
            RuleFor(x => x.PerformedByUserId)
                .NotEmpty().WithMessage("Invalid responsible user.");

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Comment must be at most 500 characters.");
        }
    }
}