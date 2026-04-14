using Eprocurement.Domain.Entities;

namespace Eprocurement.Domain.Interfaces
{
    public interface ISupplierRepository
    {
        Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default);
        Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<Supplier>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(IReadOnlyCollection<Supplier> Items, int TotalCount)> GetPagedAsync(
            string? searchTerm,
            string? documentNumber,
            bool? isActive,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
        void Update(Supplier supplier);
        void Delete(Supplier supplier);
        Task DeleteAsync(Supplier supplier, CancellationToken cancellationToken = default);
    }
}