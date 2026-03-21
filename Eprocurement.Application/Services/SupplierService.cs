using Eprocurement.Application.Abstractions;
using Eprocurement.Application.Contracts;
using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Interfaces;
using System.Collections.Generic;

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

            return new SupplierResponse(
                supplier.Id,
                supplier.CorporateName,
                supplier.DocumentNumber,
                supplier.Email,
                supplier.Phone,
                supplier.IsActive);
        }

        public async Task<IReadOnlyCollection<SupplierResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection <Supplier> suppliers = await _supplierRepository.GetAllAsync(cancellationToken);

            return suppliers
                .Select(s => new SupplierResponse(s.Id, s.CorporateName, s.DocumentNumber, s.Email, s.Phone, s.IsActive))
                .ToArray();
        }
    }
}