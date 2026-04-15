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

            if (purchaseRequest.Status != PurchaseRequestStatusEnum.InProcurement)
            {
                throw new InvalidOperationException("The request must be in procurement to generate an order.");
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
                    $"Order created for supplier {supplier.CorporateName}."),
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PurchaseOrderResponse(order.Id, order.PurchaseRequestId, order.SupplierId, order.CreatedByUserId, order.TotalAmount, order.Status.ToString());
        }

        public async Task<IReadOnlyCollection<PurchaseOrderResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<PurchaseOrder> orders = await _purchaseOrderRepository.GetAllAsync(cancellationToken);

            return orders
                .Select(Map)
                .ToArray();
        }

        public async Task<PurchaseOrderResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            PurchaseOrder? order = await _purchaseOrderRepository.GetByIdAsync(id, cancellationToken);
            return order is null ? null : Map(order);
        }

        public async Task<PurchaseOrderResponse> MarkAsSentAsync(int id, PurchaseOrderActionRequest request, CancellationToken cancellationToken = default)
        {
            PurchaseOrder order = await GetOrderOrThrowAsync(id, cancellationToken);
            User actor = await GetUserOrThrowAsync(request.PerformedByUserId, cancellationToken);

            if (order.Status != PurchaseOrderStatusEnum.Created)
            {
                throw new InvalidOperationException("Only created orders can be marked as sent.");
            }

            order.MarkAsSent();
            _purchaseOrderRepository.Update(order);

            await _purchaseHistoryRepository.AddAsync(
                new PurchaseHistory(order.PurchaseRequestId, "PurchaseOrderSentToSupplier", actor.Name, request.Comment),
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Map(order);
        }

        public async Task<PurchaseOrderResponse> MarkAsCompletedAsync(int id, PurchaseOrderActionRequest request, CancellationToken cancellationToken = default)
        {
            PurchaseOrder order = await GetOrderOrThrowAsync(id, cancellationToken);
            User actor = await GetUserOrThrowAsync(request.PerformedByUserId, cancellationToken);

            if (order.Status != PurchaseOrderStatusEnum.SentToSupplier)
            {
                throw new InvalidOperationException("Only sent orders can be completed.");
            }

            order.MarkAsCompleted();
            _purchaseOrderRepository.Update(order);

            await _purchaseHistoryRepository.AddAsync(
                new PurchaseHistory(order.PurchaseRequestId, "PurchaseOrderCompleted", actor.Name, request.Comment),
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Map(order);
        }

        public async Task<PurchaseOrderResponse> CancelAsync(int id, PurchaseOrderActionRequest request, CancellationToken cancellationToken = default)
        {
            PurchaseOrder order = await GetOrderOrThrowAsync(id, cancellationToken);
            User actor = await GetUserOrThrowAsync(request.PerformedByUserId, cancellationToken);

            if (order.Status is PurchaseOrderStatusEnum.Completed or PurchaseOrderStatusEnum.Cancelled)
            {
                throw new InvalidOperationException("Completed or cancelled orders cannot be cancelled again.");
            }

            order.Cancel();
            _purchaseOrderRepository.Update(order);

            await _purchaseHistoryRepository.AddAsync(
                new PurchaseHistory(order.PurchaseRequestId, "PurchaseOrderCancelled", actor.Name, request.Comment),
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Map(order);
        }

        private async Task<PurchaseOrder> GetOrderOrThrowAsync(int id, CancellationToken cancellationToken)
            => await _purchaseOrderRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException("Purchase order not found.");

        private async Task<User> GetUserOrThrowAsync(int userId, CancellationToken cancellationToken)
            => await _userRepository.GetByIdAsync(userId, cancellationToken)
                ?? throw new InvalidOperationException("User not found.");

        private static PurchaseOrderResponse Map(PurchaseOrder order)
            => new(order.Id, order.PurchaseRequestId, order.SupplierId, order.CreatedByUserId, order.TotalAmount, order.Status.ToString());
    }
}