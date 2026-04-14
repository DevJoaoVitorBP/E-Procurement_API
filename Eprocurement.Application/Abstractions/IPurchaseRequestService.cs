using Eprocurement.Application.Contracts;

namespace Eprocurement.Application.Abstractions
{
    public interface IPurchaseRequestService
    {
        Task<PurchaseRequestResponse> CreateAsync(CreatePurchaseRequestRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<PurchaseRequestResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<PurchaseRequestResponse> ApproveAsync(int purchaseRequestId, ApprovalDecisionRequest request, CancellationToken cancellationToken = default);
        Task<PurchaseRequestResponse> RejectAsync(int purchaseRequestId, ApprovalDecisionRequest request, CancellationToken cancellationToken = default);
        Task<PurchaseRequestResponse> MoveToProcurementAsync(int purchaseRequestId, PurchaseRequestActionRequest request, CancellationToken cancellationToken = default);
        Task<PurchaseRequestResponse?> GetByIdAsync(int purchaseRequestId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<PurchaseHistoryResponse>> GetHistoryAsync(int purchaseRequestId, CancellationToken cancellationToken = default);
    }
}