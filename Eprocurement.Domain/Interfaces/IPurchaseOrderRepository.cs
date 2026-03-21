using Eprocurement.Domain.Entities;

namespace Eprocurement.Domain.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        Task AddAsync(PurchaseOrder order, CancellationToken cancellationToken = default);
        Task<PurchaseOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<PurchaseOrder>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}