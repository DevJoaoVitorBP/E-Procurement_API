using Eprocurement.Application.Abstractions;
using Eprocurement.Application.Contracts;
using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Enums;
using Eprocurement.Domain.Interfaces;

namespace Eprocurement.Application.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly IPurchaseHistoryRepository _purchaseHistoryRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseOrderService(
            IPurchaseOrderRepository purchaseOrderRepository,
            IPurchaseRequestRepository purchaseRequestRepository,
            IPurchaseHistoryRepository purchaseHistoryRepository,
            ISupplierRepository supplierRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
            _purchaseRequestRepository = purchaseRequestRepository;
            _purchaseHistoryRepository = purchaseHistoryRepository;
            _supplierRepository = supplierRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PurchaseOrderResponse> CreateAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default)
        {
            PurchaseRequest? purchaseRequest = await _purchaseRequestRepository.GetByIdAsync(request.PurchaseRequestId, cancellationToken) ?? throw new KeyNotFoundException("Request not found.");

            Supplier supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken) ?? throw new KeyNotFoundException("Supplier not found.");

            User? creator = await _userRepository.GetByIdAsync(request.CreatedByUserId, cancellationToken) ?? throw new KeyNotFoundException("Creator user not found.");

            if (!supplier.IsActive)
            {
                throw new InvalidOperationException("Inactive supplier.");
            }

            if (purchaseRequest.Status != PurchaseRequestStatusEnum.ApprovedByManager)
            {
                throw new InvalidOperationException("The request must be approved to generate an order.");
            }

            PurchaseOrder order = new PurchaseOrder(request.PurchaseRequestId, request.SupplierId, request.CreatedByUserId, purchaseRequest.TotalAmount);

            purchaseRequest.MarkAsOrdered();

            await _purchaseOrderRepository.AddAsync(order, cancellationToken);
            _purchaseRequestRepository.Update(purchaseRequest);

            await _purchaseHistoryRepository.AddAsync(
                new PurchaseHistory(
                    purchaseRequest.Id,
                    "PurchaseOrderCreated",
                    creator.Name,
                    $"Pedido criado para fornecedor {supplier.CorporateName}."),
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PurchaseOrderResponse(order.Id, order.PurchaseRequestId, order.SupplierId, order.CreatedByUserId, order.TotalAmount, order.Status.ToString());
        }

        public async Task<IReadOnlyCollection<PurchaseOrderResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<PurchaseOrder> orders = await _purchaseOrderRepository.GetAllAsync(cancellationToken);

            return orders
                .Select(o => new PurchaseOrderResponse(o.Id, o.PurchaseRequestId, o.SupplierId, o.CreatedByUserId, o.TotalAmount, o.Status.ToString()))
                .ToArray();
        }
    }
}