using Eprocurement.Application.Abstractions;
using Eprocurement.Application.Contracts;
using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Interfaces;

namespace Eprocurement.Application.Services
{
    public class PurchaseRequestService : IPurchaseRequestService
    {
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly IPurchaseHistoryRepository _purchaseHistoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseRequestService(
            IPurchaseRequestRepository purchaseRequestRepository,
            IPurchaseHistoryRepository purchaseHistoryRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _purchaseRequestRepository = purchaseRequestRepository;
            _purchaseHistoryRepository = purchaseHistoryRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PurchaseRequestResponse> CreateAsync(CreatePurchaseRequestRequest request, CancellationToken cancellationToken = default)
        {
            User requester = await _userRepository.GetByIdAsync(request.RequestedByUserId, cancellationToken)
                ?? throw new InvalidOperationException("Requester user not found.");

            PurchaseRequest purchaseRequest = new PurchaseRequest(request.RequestedByUserId, request.Title, request.Justification);

            foreach (var item in request.Items)
            {
                purchaseRequest.AddItem(item.Description, item.Quantity, item.UnitPrice);
            }

            await _purchaseRequestRepository.AddAsync(purchaseRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _purchaseHistoryRepository.AddAsync(
                new PurchaseHistory(purchaseRequest.Id, "PurchaseRequestCreated", requester.Name, "Request created."),
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Map(purchaseRequest);
        }

        public async Task<IReadOnlyCollection<PurchaseRequestResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<PurchaseRequest> requests = await _purchaseRequestRepository.GetAllAsync(cancellationToken);

            return requests
                .Select(Map)
                .ToArray();
        }

        public async Task<PurchaseRequestResponse?> GetByIdAsync(int purchaseRequestId, CancellationToken cancellationToken = default)
        {
            PurchaseRequest? entity = await _purchaseRequestRepository.GetByIdAsync(purchaseRequestId, cancellationToken);
            return entity is null ? null : Map(entity);
        }

        public async Task<PurchaseRequestResponse> ApproveAsync(int purchaseRequestId, ApprovalDecisionRequest request, CancellationToken cancellationToken = default)
        {
            User approver = await _userRepository.GetByIdAsync(request.ApproverUserId, cancellationToken)
                ?? throw new InvalidOperationException("Approver not found.");

            PurchaseRequest purchaseRequest = await _purchaseRequestRepository.GetByIdAsync(purchaseRequestId, cancellationToken)
                ?? throw new KeyNotFoundException("Purchase request not found.");

            purchaseRequest.ApproveByManager();
            _purchaseRequestRepository.Update(purchaseRequest);

            await _purchaseHistoryRepository.AddAsync(
                new PurchaseHistory(purchaseRequest.Id, "PurchaseRequestApproved", approver.Name, request.Comment),
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Map(purchaseRequest);
        }

        public async Task<PurchaseRequestResponse> MoveToProcurementAsync(int purchaseRequestId, PurchaseRequestActionRequest request, CancellationToken cancellationToken = default)
        {
            User actor = await _userRepository.GetByIdAsync(request.PerformedByUserId, cancellationToken)
                ?? throw new InvalidOperationException("Responsible user not found.");

            PurchaseRequest purchaseRequest = await _purchaseRequestRepository.GetByIdAsync(purchaseRequestId, cancellationToken)
                ?? throw new KeyNotFoundException("Purchase request not found.");

            if (purchaseRequest.Status != Domain.Enums.PurchaseRequestStatusEnum.ApprovedByManager)
            {
                throw new InvalidOperationException("The request must be approved before moving to procurement.");
            }

            purchaseRequest.MoveToProcurement();
            _purchaseRequestRepository.Update(purchaseRequest);

            await _purchaseHistoryRepository.AddAsync(
                new PurchaseHistory(purchaseRequest.Id, "PurchaseRequestMovedToProcurement", actor.Name, request.Comment),
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Map(purchaseRequest);
        }

        public async Task<PurchaseRequestResponse> RejectAsync(int purchaseRequestId, ApprovalDecisionRequest request, CancellationToken cancellationToken = default)
        {
            User approver = await _userRepository.GetByIdAsync(request.ApproverUserId, cancellationToken)
                ?? throw new InvalidOperationException("Approver not found.");

            PurchaseRequest purchaseRequest = await _purchaseRequestRepository.GetByIdAsync(purchaseRequestId, cancellationToken)
                ?? throw new KeyNotFoundException("Purchase request not found.");

            purchaseRequest.RejectByManager();
            _purchaseRequestRepository.Update(purchaseRequest);

            await _purchaseHistoryRepository.AddAsync(
                new PurchaseHistory(purchaseRequest.Id, "PurchaseRequestRejected", approver.Name, request.Comment),
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Map(purchaseRequest);
        }

        public async Task<IReadOnlyCollection<PurchaseHistoryResponse>> GetHistoryAsync(int purchaseRequestId, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<PurchaseHistory> histories = await _purchaseHistoryRepository.GetByPurchaseRequestIdAsync(purchaseRequestId, cancellationToken);

            return histories
                .Select(x => new PurchaseHistoryResponse(
                    x.Id,
                    x.PurchaseRequestId,
                    x.Action,
                    x.PerformedBy,
                    x.Notes,
                    x.CreatedAtUtc))
                .ToArray();
        }

        private static PurchaseRequestResponse Map(PurchaseRequest entity)
            => new(entity.Id, entity.RequestedByUserId, entity.Title, entity.Justification, entity.Status.ToString(), entity.TotalAmount);
    }
}