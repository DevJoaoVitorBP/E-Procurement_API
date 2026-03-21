using Eprocurement.Domain.Entities;

namespace Eprocurement.Domain.Interfaces
{
    public interface IPurchaseHistoryRepository
    {
        Task AddAsync(PurchaseHistory history, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<PurchaseHistory>> GetByPurchaseRequestIdAsync(int purchaseRequestId, CancellationToken cancellationToken = default);
    }
}