using Eprocurement.Application.Contracts;

namespace Eprocurement.Application.Abstractions
{
    public interface ISupplierService
    {
        Task<SupplierResponse> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default);
        Task<PagedSupplierResponse> GetAllAsync(SupplierFilterRequest filter, CancellationToken cancellationToken = default);
        Task<SupplierResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<SupplierResponse> UpdateAsync(int id, UpdateSupplierRequest request, CancellationToken cancellationToken = default);
        Task<SupplierResponse> SetStatusAsync(int id, bool isActive, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}