using Eprocurement.Application.Contracts;
using FluentValidation;

namespace Eprocurement.Application.Validators
{
    public class CreatePurchaseRequestItemRequestValidator : AbstractValidator<CreatePurchaseRequestItemRequest>
    {
        public CreatePurchaseRequestItemRequestValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Descrição do item é obrigatória.")
                .MaximumLength(300);

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantidade deve ser maior que zero.");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Valor unitário deve ser maior que zero.");
        }
    }

    public class CreatePurchaseRequestRequestValidator : AbstractValidator<CreatePurchaseRequestRequest>
    {
        public CreatePurchaseRequestRequestValidator()
        {
            RuleFor(x => x.RequestedByUserId)
                .GreaterThan(0).WithMessage("Solicitante inválido.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Título é obrigatório.")
                .MaximumLength(200);

            RuleFor(x => x.Justification)
                .NotEmpty().WithMessage("Justificativa é obrigatória.")
                .MaximumLength(1000);

            RuleFor(x => x.Items)
                .NotNull().WithMessage("Itens são obrigatórios.")
                .Must(x => x.Count > 0).WithMessage("Informe ao menos um item.");

            RuleForEach(x => x.Items)
                .SetValidator(new CreatePurchaseRequestItemRequestValidator());
        }
    }

    public class ApprovalDecisionRequestValidator : AbstractValidator<ApprovalDecisionRequest>
    {
        public ApprovalDecisionRequestValidator()
        {
            RuleFor(x => x.ApproverUserId)
                .GreaterThan(0).WithMessage("Aprovador inválido.");

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Comentário deve ter no máximo 500 caracteres.");
        }
    }
}