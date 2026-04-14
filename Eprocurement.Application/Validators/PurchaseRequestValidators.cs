using Eprocurement.Application.Contracts;
using FluentValidation;

namespace Eprocurement.Application.Validators
{
    public class CreatePurchaseRequestItemRequestValidator : AbstractValidator<CreatePurchaseRequestItemRequest>
    {
        public CreatePurchaseRequestItemRequestValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Item description is required.")
                .MaximumLength(300);

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Unit price must be greater than zero.");
        }
    }

    public class CreatePurchaseRequestRequestValidator : AbstractValidator<CreatePurchaseRequestRequest>
    {
        public CreatePurchaseRequestRequestValidator()
        {
            RuleFor(x => x.RequestedByUserId)
                .NotEmpty().WithMessage("Invalid requester.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200);

            RuleFor(x => x.Justification)
                .NotEmpty().WithMessage("Justification is required.")
                .MaximumLength(1000);

            RuleFor(x => x.Items)
                .NotNull().WithMessage("Items are required.")
                .Must(x => x.Count > 0).WithMessage("Provide at least one item.");

            RuleForEach(x => x.Items)
                .SetValidator(new CreatePurchaseRequestItemRequestValidator());
        }
    }

    /// <summary>
    /// Validator for approval decision requests.
    /// </summary>
    public class ApprovalDecisionRequestValidator : AbstractValidator<ApprovalDecisionRequest>
    {
        public ApprovalDecisionRequestValidator()
        {
            RuleFor(x => x.ApproverUserId)
                .NotEmpty().WithMessage("Invalid approver.");

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Comment must be at most 500 characters.");
        }
    }

    public class PurchaseRequestActionRequestValidator : AbstractValidator<PurchaseRequestActionRequest>
    {
        public PurchaseRequestActionRequestValidator()
        {
            RuleFor(x => x.PerformedByUserId)
                .NotEmpty().WithMessage("Invalid responsible user.");

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Comment must be at most 500 characters.");
        }
    }
}