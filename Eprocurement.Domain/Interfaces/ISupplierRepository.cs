using Eprocurement.Domain.Entities;

namespace Eprocurement.Domain.Interfaces
{
    public interface ISupplierRepository
    {
        Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default);
        Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<Supplier>> GetAllAsync(CancellationToken cancellationToken = default);
        void Update(Supplier supplier);
        void Remove(Supplier supplier);
    }
}