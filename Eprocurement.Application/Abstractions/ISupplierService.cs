using Eprocurement.Application.Contracts;

namespace Eprocurement.Application.Abstractions
{
    public interface ISupplierService
    {
        Task<SupplierResponse> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<SupplierResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}