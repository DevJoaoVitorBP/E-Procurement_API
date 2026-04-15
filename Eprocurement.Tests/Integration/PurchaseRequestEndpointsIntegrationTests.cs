using System.Net;
using System.Net.Http.Json;
using Eprocurement.Application.Abstractions;
using Eprocurement.Application.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Eprocurement.Tests.Integration;

public class PurchaseRequestEndpointsIntegrationTests
{
    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        FakePurchaseRequestService fakeService = new();
        await using TestApiFactory factory = new(fakeService);
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

        HttpResponseMessage response = await client.GetAsync("/api/purchaserequest");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        IReadOnlyCollection<PurchaseRequestResponse>? payload = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<PurchaseRequestResponse>>();
        Assert.NotNull(payload);
        Assert.Single(payload);
    }

    [Fact]
    public async Task Approve_WhenServiceThrowsNotFound_ShouldReturnNotFound()
    {
        FakePurchaseRequestService fakeService = new() { ThrowOnApprove = new KeyNotFoundException("Purchase request not found.") };
        await using TestApiFactory factory = new(fakeService);
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/purchaserequest/99/approve", new ApprovalDecisionRequest(2, "ok"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed class TestApiFactory : WebApplicationFactory<EprocurementApi.Program>
    {
        private readonly IPurchaseRequestService _service;

        public TestApiFactory(IPurchaseRequestService service)
        {
            _service = service;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IPurchaseRequestService>();
                services.AddSingleton(_service);

                services
                    .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                        options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
            });
        }
    }

    private sealed class FakePurchaseRequestService : IPurchaseRequestService
    {
        public Exception? ThrowOnApprove { get; set; }

        public Task<PurchaseRequestResponse> ApproveAsync(int purchaseRequestId, ApprovalDecisionRequest request, CancellationToken cancellationToken = default)
            => ThrowOnApprove is null
                ? Task.FromResult(new PurchaseRequestResponse(purchaseRequestId, 1, "Title", "Justification", "ApprovedByManager", 100))
                : Task.FromException<PurchaseRequestResponse>(ThrowOnApprove);

        public Task<PurchaseRequestResponse> CreateAsync(CreatePurchaseRequestRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new PurchaseRequestResponse(1, request.RequestedByUserId, request.Title, request.Justification, "PendingManagerApproval", 100));

        public Task<IReadOnlyCollection<PurchaseRequestResponse>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<PurchaseRequestResponse>>([new PurchaseRequestResponse(1, 1, "Title", "Justification", "PendingManagerApproval", 100)]);

        public Task<PurchaseRequestResponse?> GetByIdAsync(int purchaseRequestId, CancellationToken cancellationToken = default)
            => Task.FromResult<PurchaseRequestResponse?>(new PurchaseRequestResponse(purchaseRequestId, 1, "Title", "Justification", "PendingManagerApproval", 100));

        public Task<IReadOnlyCollection<PurchaseHistoryResponse>> GetHistoryAsync(int purchaseRequestId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<PurchaseHistoryResponse>>([new PurchaseHistoryResponse(1, purchaseRequestId, "PurchaseRequestCreated", "Requester", "created", DateTime.UtcNow)]);

        public Task<PurchaseRequestResponse> MoveToProcurementAsync(int purchaseRequestId, PurchaseRequestActionRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new PurchaseRequestResponse(purchaseRequestId, 1, "Title", "Justification", "InProcurement", 100));

        public Task<PurchaseRequestResponse> RejectAsync(int purchaseRequestId, ApprovalDecisionRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new PurchaseRequestResponse(purchaseRequestId, 1, "Title", "Justification", "RejectedByManager", 100));
    }
}
