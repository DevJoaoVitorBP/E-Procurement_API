using Eprocurement.Application.Contracts;

namespace Eprocurement.Application.Abstractions
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrderResponse> CreateAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<PurchaseOrderResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<PurchaseOrderResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PurchaseOrderResponse> MarkAsSentAsync(int id, PurchaseOrderActionRequest request, CancellationToken cancellationToken = default);
        Task<PurchaseOrderResponse> MarkAsCompletedAsync(int id, PurchaseOrderActionRequest request, CancellationToken cancellationToken = default);
        Task<PurchaseOrderResponse> CancelAsync(int id, PurchaseOrderActionRequest request, CancellationToken cancellationToken = default);
    }
}