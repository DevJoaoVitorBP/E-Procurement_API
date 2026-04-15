using Eprocurement.Application.Abstractions;
using Eprocurement.Application.Contracts;
using EprocurementApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Eprocurement.Tests.Api;

public class PurchaseOrderControllerTests
{
    [Fact]
    public async Task GetAll_ShouldReturnOkWithPayload()
    {
        FakePurchaseOrderService service = new()
        {
            AllResponses = [new PurchaseOrderResponse(1, 1, 1, 3, 100, "Created")]
        };
        PurchaseOrderController controller = new(service, NullLogger<PurchaseOrderController>.Instance);

        IActionResult result = await controller.GetAll(CancellationToken.None);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        IReadOnlyCollection<PurchaseOrderResponse> payload = Assert.IsAssignableFrom<IReadOnlyCollection<PurchaseOrderResponse>>(ok.Value);
        Assert.Single(payload);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturnNotFound()
    {
        FakePurchaseOrderService service = new() { ResponseById = null };
        PurchaseOrderController controller = new(service, NullLogger<PurchaseOrderController>.Instance);

        IActionResult result = await controller.GetById(10, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_WhenServiceThrowsInvalidOperation_ShouldReturnBadRequest()
    {
        const string expectedMessage = "Invalid create operation.";
        FakePurchaseOrderService service = new() { ThrowOnCreate = new InvalidOperationException(expectedMessage) };
        PurchaseOrderController controller = new(service, NullLogger<PurchaseOrderController>.Instance);

        IActionResult result = await controller.Create(new CreatePurchaseOrderRequest(1, 1, 3), CancellationToken.None);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(expectedMessage, GetMessage(badRequest));
    }

    [Fact]
    public async Task GetById_WhenFound_ShouldReturnOk()
    {
        FakePurchaseOrderService service = new()
        {
            ResponseById = new PurchaseOrderResponse(1, 1, 1, 3, 100, "Created")
        };
        PurchaseOrderController controller = new(service, NullLogger<PurchaseOrderController>.Instance);

        IActionResult result = await controller.GetById(1, CancellationToken.None);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        PurchaseOrderResponse payload = Assert.IsType<PurchaseOrderResponse>(ok.Value);
        Assert.Equal("Created", payload.Status);
    }

    [Fact]
    public async Task Create_WhenServiceThrowsKeyNotFound_ShouldReturnNotFound()
    {
        const string expectedMessage = "Request not found.";
        FakePurchaseOrderService service = new() { ThrowOnCreate = new KeyNotFoundException(expectedMessage) };
        PurchaseOrderController controller = new(service, NullLogger<PurchaseOrderController>.Instance);

        IActionResult result = await controller.Create(new CreatePurchaseOrderRequest(1, 1, 3), CancellationToken.None);

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(expectedMessage, GetMessage(notFound));
    }

    [Fact]
    public async Task MarkAsSent_WhenServiceThrowsInvalidOperation_ShouldReturnBadRequest()
    {
        const string expectedMessage = "Invalid send operation.";
        FakePurchaseOrderService service = new() { ThrowOnSent = new InvalidOperationException(expectedMessage) };
        PurchaseOrderController controller = new(service, NullLogger<PurchaseOrderController>.Instance);

        IActionResult result = await controller.MarkAsSent(1, new PurchaseOrderActionRequest(3, "send"), CancellationToken.None);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(expectedMessage, GetMessage(badRequest));
    }

    [Fact]
    public async Task MarkAsSent_WhenServiceThrowsKeyNotFound_ShouldReturnNotFound()
    {
        const string expectedMessage = "Purchase order not found.";
        FakePurchaseOrderService service = new() { ThrowOnSent = new KeyNotFoundException(expectedMessage) };
        PurchaseOrderController controller = new(service, NullLogger<PurchaseOrderController>.Instance);

        IActionResult result = await controller.MarkAsSent(1, new PurchaseOrderActionRequest(3, "send"), CancellationToken.None);

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(expectedMessage, GetMessage(notFound));
    }

    [Fact]
    public async Task MarkAsSent_WhenServiceReturnsResponse_ShouldReturnOk()
    {
        FakePurchaseOrderService service = new()
        {
            CreateResponse = new PurchaseOrderResponse(1, 1, 1, 3, 100, "SentToSupplier")
        };
        PurchaseOrderController controller = new(service, NullLogger<PurchaseOrderController>.Instance);

        IActionResult result = await controller.MarkAsSent(1, new PurchaseOrderActionRequest(3, "send"), CancellationToken.None);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        PurchaseOrderResponse payload = Assert.IsType<PurchaseOrderResponse>(ok.Value);
        Assert.Equal("SentToSupplier", payload.Status);
    }

    [Fact]
    public async Task MarkAsCompleted_WhenServiceThrowsInvalidOperation_ShouldReturnBadRequest()
    {
        const string expectedMessage = "Invalid complete operation.";
        FakePurchaseOrderService service = new() { ThrowOnCompleted = new InvalidOperationException(expectedMessage) };
        PurchaseOrderController controller = new(service, NullLogger<PurchaseOrderController>.Instance);

        IActionResult result = await controller.MarkAsCompleted(1, new PurchaseOrderActionRequest(3, "complete"), CancellationToken.None);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(expectedMessage, GetMessage(badRequest));
    }

    [Fact]
    public async Task MarkAsCompleted_WhenServiceThrowsKeyNotFound_ShouldReturnNotFound()
    {
        const string expectedMessage = "Purchase order not found.";
        FakePurchaseOrderService service = new() { ThrowOnCompleted = new KeyNotFoundException(expectedMessage) };
        PurchaseOrderController controller = new(service, NullLogger<PurchaseOrderController>.Instance);

        IActionResult result = await controller.MarkAsCompleted(1, new PurchaseOrderActionRequest(3, "complete"), CancellationToken.None);

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(expectedMessage, GetMessage(notFound));
    }

    [Fact]
    public async Task MarkAsCompleted_WhenServiceReturnsResponse_ShouldReturnOk()
    {
        FakePurchaseOrderService service = new()
        {
            CreateResponse = new PurchaseOrderResponse(1, 1, 1, 3, 100, "Completed")
        };
        PurchaseOrderController controller = new(service, NullLogger<PurchaseOrderController>.Instance);

        IActionResult result = await controller.MarkAsCompleted(1, new PurchaseOrderActionRequest(3, "complete"), CancellationToken.None);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        PurchaseOrderResponse payload = Assert.IsType<PurchaseOrderResponse>(ok.Value);
        Assert.Equal("Completed", payload.Status);
    }

    [Fact]
    public async Task Cancel_WhenServiceThrowsKeyNotFound_ShouldReturnNotFound()
    {
        const string expectedMessage = "Purchase order not found.";
        FakePurchaseOrderService service = new() { ThrowOnCancel = new KeyNotFoundException(expectedMessage) };
        PurchaseOrderController controller = new(service, NullLogger<PurchaseOrderController>.Instance);

        IActionResult result = await controller.Cancel(1, new PurchaseOrderActionRequest(3, "cancel"), CancellationToken.None);

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(expectedMessage, GetMessage(notFound));
    }

    [Fact]
    public async Task Cancel_WhenServiceThrowsInvalidOperation_ShouldReturnBadRequest()
    {
        const string expectedMessage = "Invalid cancel operation.";
        FakePurchaseOrderService service = new() { ThrowOnCancel = new InvalidOperationException(expectedMessage) };
        PurchaseOrderController controller = new(service, NullLogger<PurchaseOrderController>.Instance);

        IActionResult result = await controller.Cancel(1, new PurchaseOrderActionRequest(3, "cancel"), CancellationToken.None);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(expectedMessage, GetMessage(badRequest));
    }

    [Fact]
    public async Task Cancel_WhenServiceReturnsResponse_ShouldReturnOk()
    {
        FakePurchaseOrderService service = new()
        {
            CreateResponse = new PurchaseOrderResponse(1, 1, 1, 3, 100, "Cancelled")
        };
        PurchaseOrderController controller = new(service, NullLogger<PurchaseOrderController>.Instance);

        IActionResult result = await controller.Cancel(1, new PurchaseOrderActionRequest(3, "cancel"), CancellationToken.None);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        PurchaseOrderResponse payload = Assert.IsType<PurchaseOrderResponse>(ok.Value);
        Assert.Equal("Cancelled", payload.Status);
    }

    [Fact]
    public async Task Create_WhenServiceReturnsResponse_ShouldReturnCreatedAtAction()
    {
        FakePurchaseOrderService service = new()
        {
            CreateResponse = new PurchaseOrderResponse(1, 1, 1, 3, 100, "Created")
        };
        PurchaseOrderController controller = new(service, NullLogger<PurchaseOrderController>.Instance);

        IActionResult result = await controller.Create(new CreatePurchaseOrderRequest(1, 1, 3), CancellationToken.None);

        CreatedAtActionResult created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(PurchaseOrderController.GetById), created.ActionName);
        Assert.Equal(1, created.RouteValues?["id"]);
        PurchaseOrderResponse payload = Assert.IsType<PurchaseOrderResponse>(created.Value);
        Assert.Equal("Created", payload.Status);
    }

    private static string? GetMessage(ObjectResult result)
        => result.Value?.GetType().GetProperty("message")?.GetValue(result.Value)?.ToString();

    private sealed class FakePurchaseOrderService : IPurchaseOrderService
    {
        public PurchaseOrderResponse? CreateResponse { get; set; }
        public PurchaseOrderResponse? ResponseById { get; set; }
        public IReadOnlyCollection<PurchaseOrderResponse> AllResponses { get; set; } = [];
        public Exception? ThrowOnCreate { get; set; }
        public Exception? ThrowOnSent { get; set; }
        public Exception? ThrowOnCompleted { get; set; }
        public Exception? ThrowOnCancel { get; set; }

        public Task<PurchaseOrderResponse> CancelAsync(int id, PurchaseOrderActionRequest request, CancellationToken cancellationToken = default)
            => ThrowOnCancel is null
                ? Task.FromResult(CreateResponse ?? new PurchaseOrderResponse(1, 1, 1, 3, 100, "Cancelled"))
                : Task.FromException<PurchaseOrderResponse>(ThrowOnCancel);

        public Task<PurchaseOrderResponse> CreateAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default)
            => ThrowOnCreate is null ? Task.FromResult(CreateResponse ?? new PurchaseOrderResponse(1, request.PurchaseRequestId, request.SupplierId, request.CreatedByUserId, 100, "Created")) : Task.FromException<PurchaseOrderResponse>(ThrowOnCreate);

        public Task<IReadOnlyCollection<PurchaseOrderResponse>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(AllResponses);

        public Task<PurchaseOrderResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(ResponseById);

        public Task<PurchaseOrderResponse> MarkAsCompletedAsync(int id, PurchaseOrderActionRequest request, CancellationToken cancellationToken = default)
            => ThrowOnCompleted is null
                ? Task.FromResult(CreateResponse ?? new PurchaseOrderResponse(1, 1, 1, 3, 100, "Completed"))
                : Task.FromException<PurchaseOrderResponse>(ThrowOnCompleted);

        public Task<PurchaseOrderResponse> MarkAsSentAsync(int id, PurchaseOrderActionRequest request, CancellationToken cancellationToken = default)
            => ThrowOnSent is null ? Task.FromResult(CreateResponse ?? new PurchaseOrderResponse(1, 1, 1, 3, 100, "SentToSupplier")) : Task.FromException<PurchaseOrderResponse>(ThrowOnSent);
    }
}
