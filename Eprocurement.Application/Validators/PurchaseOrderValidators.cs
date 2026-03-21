using Eprocurement.Application.Contracts;
using FluentValidation;

namespace Eprocurement.Application.Validators
{
    public class CreatePurchaseOrderRequestValidator : AbstractValidator<CreatePurchaseOrderRequest>
    {
        public CreatePurchaseOrderRequestValidator()
        {
            RuleFor(x => x.PurchaseRequestId)
                .GreaterThan(0).WithMessage("Requisição de compra inválida.");

            RuleFor(x => x.SupplierId)
                .GreaterThan(0).WithMessage("Fornecedor inválido.");

            RuleFor(x => x.CreatedByUserId)
                .GreaterThan(0).WithMessage("Usuário criador inválido.");
        }
    }
}