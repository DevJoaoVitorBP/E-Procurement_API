using Eprocurement.Domain.Entities;

namespace Eprocurement.Domain.Interfaces
{
    public interface IPurchaseRequestRepository
    {
        Task AddAsync(PurchaseRequest request, CancellationToken cancellationToken = default);
        Task<PurchaseRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<PurchaseRequest>> GetAllAsync(CancellationToken cancellationToken = default);
        void Update(PurchaseRequest request);
    }
}