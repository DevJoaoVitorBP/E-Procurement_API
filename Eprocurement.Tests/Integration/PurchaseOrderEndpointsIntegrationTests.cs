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

public class PurchaseOrderEndpointsIntegrationTests
{
    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        FakePurchaseOrderService fakeService = new();
        await using TestApiFactory factory = new(fakeService);
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

        HttpResponseMessage response = await client.GetAsync("/api/purchaseorder");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        IReadOnlyCollection<PurchaseOrderResponse>? payload = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<PurchaseOrderResponse>>();
        Assert.NotNull(payload);
        Assert.Single(payload);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        FakePurchaseOrderService fakeService = new();
        await using TestApiFactory factory = new(fakeService);
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/purchaseorder",
            new CreatePurchaseOrderRequest(1, 1, 3));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private sealed class TestApiFactory : WebApplicationFactory<EprocurementApi.Program>
    {
        private readonly IPurchaseOrderService _service;

        public TestApiFactory(IPurchaseOrderService service)
        {
            _service = service;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IPurchaseOrderService>();
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

    private sealed class FakePurchaseOrderService : IPurchaseOrderService
    {
        public Task<PurchaseOrderResponse> CancelAsync(int id, PurchaseOrderActionRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new PurchaseOrderResponse(id, 1, 1, request.PerformedByUserId, 100, "Cancelled"));

        public Task<PurchaseOrderResponse> CreateAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new PurchaseOrderResponse(1, request.PurchaseRequestId, request.SupplierId, request.CreatedByUserId, 100, "Created"));

        public Task<IReadOnlyCollection<PurchaseOrderResponse>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<PurchaseOrderResponse>>([new PurchaseOrderResponse(1, 1, 1, 3, 100, "Created")]);

        public Task<PurchaseOrderResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult<PurchaseOrderResponse?>(new PurchaseOrderResponse(id, 1, 1, 3, 100, "Created"));

        public Task<PurchaseOrderResponse> MarkAsCompletedAsync(int id, PurchaseOrderActionRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new PurchaseOrderResponse(id, 1, 1, request.PerformedByUserId, 100, "Completed"));

        public Task<PurchaseOrderResponse> MarkAsSentAsync(int id, PurchaseOrderActionRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new PurchaseOrderResponse(id, 1, 1, request.PerformedByUserId, 100, "SentToSupplier"));
    }
}
