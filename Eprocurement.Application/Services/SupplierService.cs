using Eprocurement.Application.Abstractions;
using Eprocurement.Application.Contracts;
using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Interfaces;

namespace Eprocurement.Application.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SupplierService(ISupplierRepository supplierRepository, IUnitOfWork unitOfWork)
        {
            _supplierRepository = supplierRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<SupplierResponse> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default)
        {
            Supplier supplier = new Supplier(request.CorporateName, request.DocumentNumber, request.Email, request.Phone);

            await _supplierRepository.AddAsync(supplier, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToResponse(supplier);
        }

        public async Task<PagedSupplierResponse> GetAllAsync(SupplierFilterRequest filter, CancellationToken cancellationToken = default)
        {
            int page = filter.Page <= 0 ? 1 : filter.Page;
            int pageSize = filter.PageSize <= 0 ? 10 : Math.Min(filter.PageSize, 100);

            (IReadOnlyCollection<Supplier> items, int totalCount) = await _supplierRepository.GetPagedAsync(
                filter.SearchTerm,
                filter.DocumentNumber,
                filter.IsActive,
                page,
                pageSize,
                cancellationToken);

            SupplierResponse[] suppliers = items
                .Select(ToResponse)
                .ToArray();

            int totalPages = totalCount == 0
                ? 0
                : (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PagedSupplierResponse(
                suppliers,
                page,
                pageSize,
                totalCount,
                totalPages);
        }

        public async Task<SupplierResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Supplier supplier = await GetSupplierOrThrowAsync(id, cancellationToken);
            return ToResponse(supplier);
        }

        public async Task<SupplierResponse> UpdateAsync(int id, UpdateSupplierRequest request, CancellationToken cancellationToken = default)
        {
            Supplier supplier = await GetSupplierOrThrowAsync(id, cancellationToken);

            supplier.UpdateData(request.CorporateName, request.Email, request.Phone);
            _supplierRepository.Update(supplier);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToResponse(supplier);
        }

        public async Task<SupplierResponse> SetStatusAsync(int id, bool isActive, CancellationToken cancellationToken = default)
        {
            Supplier supplier = await GetSupplierOrThrowAsync(id, cancellationToken);

            if (isActive)
            {
                supplier.Activate();
            }
            else
            {
                supplier.Deactivate();
            }

            _supplierRepository.Update(supplier);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToResponse(supplier);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            Supplier supplier = await GetSupplierOrThrowAsync(id, cancellationToken);

            await _supplierRepository.DeleteAsync(supplier, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<Supplier> GetSupplierOrThrowAsync(int id, CancellationToken cancellationToken)
        {
            Supplier? supplier = await _supplierRepository.GetByIdAsync(id, cancellationToken);
            if (supplier is null)
            {
                throw new InvalidOperationException("Supplier not found.");
            }

            return supplier;
        }

        private static SupplierResponse ToResponse(Supplier supplier)
            => new(
                supplier.Id,
                supplier.CorporateName,
                supplier.DocumentNumber,
                supplier.Email,
                supplier.Phone,
                supplier.IsActive);
    }
}