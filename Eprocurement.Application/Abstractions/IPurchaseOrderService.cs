using Eprocurement.Application.Contracts;

namespace Eprocurement.Application.Abstractions
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrderResponse> CreateAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<PurchaseOrderResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}