using Eprocurement.Application.Abstractions;
using Eprocurement.Application.Contracts;
using EprocurementApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Eprocurement.Tests.Api;

public class PurchaseRequestControllerTests
{
    [Fact]
    public async Task GetAll_ShouldReturnOkWithPayload()
    {
        FakePurchaseRequestService service = new()
        {
            AllResponses = [new PurchaseRequestResponse(1, 1, "Title", "Justification", "PendingManagerApproval", 100)]
        };
        PurchaseRequestController controller = new(service, NullLogger<PurchaseRequestController>.Instance);

        IActionResult result = await controller.GetAll(CancellationToken.None);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        IReadOnlyCollection<PurchaseRequestResponse> payload = Assert.IsAssignableFrom<IReadOnlyCollection<PurchaseRequestResponse>>(ok.Value);
        Assert.Single(payload);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturnNotFound()
    {
        FakePurchaseRequestService service = new() { ResponseById = null };
        PurchaseRequestController controller = new(service, NullLogger<PurchaseRequestController>.Instance);

        IActionResult result = await controller.GetById(10, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WhenFound_ShouldReturnOk()
    {
        FakePurchaseRequestService service = new()
        {
            ResponseById = new PurchaseRequestResponse(1, 1, "Title", "Justification", "PendingManagerApproval", 100)
        };
        PurchaseRequestController controller = new(service, NullLogger<PurchaseRequestController>.Instance);

        IActionResult result = await controller.GetById(1, CancellationToken.None);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        PurchaseRequestResponse payload = Assert.IsType<PurchaseRequestResponse>(ok.Value);
        Assert.Equal(1, payload.Id);
    }

    [Fact]
    public async Task Approve_WhenServiceThrowsKeyNotFound_ShouldReturnNotFound()
    {
        FakePurchaseRequestService service = new() { ThrowOnApprove = new KeyNotFoundException("Purchase request not found.") };
        PurchaseRequestController controller = new(service, NullLogger<PurchaseRequestController>.Instance);

        IActionResult result = await controller.Approve(1, new ApprovalDecisionRequest(2, "ok"), CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Approve_WhenServiceThrowsInvalidOperation_ShouldReturnBadRequest()
    {
        FakePurchaseRequestService service = new() { ThrowOnApprove = new InvalidOperationException("Invalid approval.") };
        PurchaseRequestController controller = new(service, NullLogger<PurchaseRequestController>.Instance);

        IActionResult result = await controller.Approve(1, new ApprovalDecisionRequest(2, "ok"), CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Reject_WhenServiceThrowsKeyNotFound_ShouldReturnNotFound()
    {
        FakePurchaseRequestService service = new() { ThrowOnReject = new KeyNotFoundException("Purchase request not found.") };
        PurchaseRequestController controller = new(service, NullLogger<PurchaseRequestController>.Instance);

        IActionResult result = await controller.Reject(1, new ApprovalDecisionRequest(2, "reject"), CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Reject_WhenServiceThrowsInvalidOperation_ShouldReturnBadRequest()
    {
        FakePurchaseRequestService service = new() { ThrowOnReject = new InvalidOperationException("Invalid rejection.") };
        PurchaseRequestController controller = new(service, NullLogger<PurchaseRequestController>.Instance);

        IActionResult result = await controller.Reject(1, new ApprovalDecisionRequest(2, "reject"), CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task MoveToProcurement_WhenServiceThrowsInvalidOperation_ShouldReturnBadRequest()
    {
        FakePurchaseRequestService service = new() { ThrowOnMove = new InvalidOperationException("Invalid transition.") };
        PurchaseRequestController controller = new(service, NullLogger<PurchaseRequestController>.Instance);

        IActionResult result = await controller.MoveToProcurement(1, new PurchaseRequestActionRequest(3, "move"), CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task MoveToProcurement_WhenServiceThrowsKeyNotFound_ShouldReturnNotFound()
    {
        FakePurchaseRequestService service = new() { ThrowOnMove = new KeyNotFoundException("Purchase request not found.") };
        PurchaseRequestController controller = new(service, NullLogger<PurchaseRequestController>.Instance);

        IActionResult result = await controller.MoveToProcurement(1, new PurchaseRequestActionRequest(3, "move"), CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_WhenServiceReturnsResponse_ShouldReturnCreatedAtAction()
    {
        FakePurchaseRequestService service = new()
        {
            CreateResponse = new PurchaseRequestResponse(1, 1, "Title", "Justification", "PendingManagerApproval", 100)
        };
        PurchaseRequestController controller = new(service, NullLogger<PurchaseRequestController>.Instance);

        IActionResult result = await controller.Create(
            new CreatePurchaseRequestRequest(1, "Title", "Justification", [new CreatePurchaseRequestItemRequest("Item", 1, 100)]),
            CancellationToken.None);

        CreatedAtActionResult created = Assert.IsType<CreatedAtActionResult>(result);
        PurchaseRequestResponse payload = Assert.IsType<PurchaseRequestResponse>(created.Value);
        Assert.Equal(1, payload.Id);
    }

    [Fact]
    public async Task GetHistory_ShouldReturnOkWithPayload()
    {
        FakePurchaseRequestService service = new()
        {
            HistoryResponses = [new PurchaseHistoryResponse(1, 1, "PurchaseRequestCreated", "Requester", "created", DateTime.UtcNow)]
        };
        PurchaseRequestController controller = new(service, NullLogger<PurchaseRequestController>.Instance);

        IActionResult result = await controller.GetHistory(1, CancellationToken.None);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        IReadOnlyCollection<PurchaseHistoryResponse> payload = Assert.IsAssignableFrom<IReadOnlyCollection<PurchaseHistoryResponse>>(ok.Value);
        Assert.Single(payload);
    }

    private sealed class FakePurchaseRequestService : IPurchaseRequestService
    {
        public PurchaseRequestResponse? CreateResponse { get; set; }
        public PurchaseRequestResponse? ResponseById { get; set; }
        public IReadOnlyCollection<PurchaseRequestResponse> AllResponses { get; set; } = [];
        public IReadOnlyCollection<PurchaseHistoryResponse> HistoryResponses { get; set; } = [];
        public Exception? ThrowOnApprove { get; set; }
        public Exception? ThrowOnReject { get; set; }
        public Exception? ThrowOnMove { get; set; }

        public Task<PurchaseRequestResponse> ApproveAsync(int purchaseRequestId, ApprovalDecisionRequest request, CancellationToken cancellationToken = default)
            => ThrowOnApprove is null ? Task.FromResult(CreateResponse ?? new PurchaseRequestResponse(1, 1, "T", "J", "ApprovedByManager", 10)) : Task.FromException<PurchaseRequestResponse>(ThrowOnApprove);

        public Task<PurchaseRequestResponse> CreateAsync(CreatePurchaseRequestRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(CreateResponse ?? new PurchaseRequestResponse(1, request.RequestedByUserId, request.Title, request.Justification, "PendingManagerApproval", 0));

        public Task<IReadOnlyCollection<PurchaseRequestResponse>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(AllResponses);

        public Task<PurchaseRequestResponse?> GetByIdAsync(int purchaseRequestId, CancellationToken cancellationToken = default)
            => Task.FromResult(ResponseById);

        public Task<IReadOnlyCollection<PurchaseHistoryResponse>> GetHistoryAsync(int purchaseRequestId, CancellationToken cancellationToken = default)
            => Task.FromResult(HistoryResponses);

        public Task<PurchaseRequestResponse> MoveToProcurementAsync(int purchaseRequestId, PurchaseRequestActionRequest request, CancellationToken cancellationToken = default)
            => ThrowOnMove is null ? Task.FromResult(CreateResponse ?? new PurchaseRequestResponse(1, 1, "T", "J", "InProcurement", 10)) : Task.FromException<PurchaseRequestResponse>(ThrowOnMove);

        public Task<PurchaseRequestResponse> RejectAsync(int purchaseRequestId, ApprovalDecisionRequest request, CancellationToken cancellationToken = default)
            => ThrowOnReject is null ? Task.FromResult(CreateResponse ?? new PurchaseRequestResponse(1, 1, "T", "J", "RejectedByManager", 10)) : Task.FromException<PurchaseRequestResponse>(ThrowOnReject);
    }
}
