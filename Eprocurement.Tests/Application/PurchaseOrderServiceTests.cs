using Eprocurement.Application.Contracts;
using Eprocurement.Application.Services;
using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Enums;
using Eprocurement.Domain.Interfaces;
using Xunit;

namespace Eprocurement.Tests.Application;

public class PurchaseOrderServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenRequestIsNotInProcurement_ShouldThrow()
    {
        PurchaseRequest request = new(1, "Title", "Justification");
        request.ApproveByManager();

        FakePurchaseRequestRepository purchaseRequestRepository = new() { RequestById = request };
        FakeSupplierRepository supplierRepository = new();
        supplierRepository.SupplierById = new Supplier("Tech", "123", "tech@corp.com", "+55 11 1111-1111");

        FakeUserRepository userRepository = new();
        userRepository.UsersById[3] = new User("Creator", "creator@eproc.com", "123456", UserRolesEnum.Procurement);

        PurchaseOrderService service = new(
            new FakePurchaseOrderRepository(),
            purchaseRequestRepository,
            new FakePurchaseHistoryRepository(),
            supplierRepository,
            userRepository,
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(new CreatePurchaseOrderRequest(1, 1, 3), CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_WhenValid_ShouldCreateOrderAndMarkRequestAsOrdered()
    {
        PurchaseRequest request = new(1, "Title", "Justification");
        request.ApproveByManager();
        request.MoveToProcurement();
        request.AddItem("Notebook", 1, 5000);

        FakePurchaseRequestRepository purchaseRequestRepository = new() { RequestById = request };

        Supplier supplier = new("Tech", "123", "tech@corp.com", "+55 11 1111-1111");
        FakeSupplierRepository supplierRepository = new() { SupplierById = supplier };

        FakeUserRepository userRepository = new();
        userRepository.UsersById[3] = new User("Creator", "creator@eproc.com", "123456", UserRolesEnum.Procurement);

        FakePurchaseOrderRepository purchaseOrderRepository = new();
        FakePurchaseHistoryRepository historyRepository = new();
        FakeUnitOfWork unitOfWork = new();

        PurchaseOrderService service = new(
            purchaseOrderRepository,
            purchaseRequestRepository,
            historyRepository,
            supplierRepository,
            userRepository,
            unitOfWork);

        PurchaseOrderResponse response = await service.CreateAsync(new CreatePurchaseOrderRequest(1, 1, 3), CancellationToken.None);

        Assert.NotNull(purchaseOrderRepository.AddedOrder);
        Assert.Equal(PurchaseOrderStatusEnum.Created.ToString(), response.Status);
        Assert.Equal(PurchaseRequestStatusEnum.Ordered, request.Status);
        Assert.True(purchaseRequestRepository.UpdateCalled);
        Assert.Single(historyRepository.AddedHistories);
        Assert.Equal("PurchaseOrderCreated", historyRepository.AddedHistories[0].Action);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task MarkAsSentAsync_WhenOrderIsAlreadySent_ShouldThrow()
    {
        PurchaseOrder order = new(1, 1, 3, 100);
        order.MarkAsSent();

        FakePurchaseOrderRepository orderRepository = new() { OrderById = order };
        FakeUserRepository userRepository = new();
        userRepository.UsersById[3] = new User("Actor", "actor@eproc.com", "123456", UserRolesEnum.Procurement);

        PurchaseOrderService service = new(
            orderRepository,
            new FakePurchaseRequestRepository(),
            new FakePurchaseHistoryRepository(),
            new FakeSupplierRepository(),
            userRepository,
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.MarkAsSentAsync(1, new PurchaseOrderActionRequest(3, "Send"), CancellationToken.None));
    }

    [Fact]
    public async Task CancelAsync_WhenOrderIsCompleted_ShouldThrow()
    {
        PurchaseOrder order = new(1, 1, 3, 100);
        order.MarkAsSent();
        order.MarkAsCompleted();

        FakePurchaseOrderRepository orderRepository = new() { OrderById = order };
        FakeUserRepository userRepository = new();
        userRepository.UsersById[3] = new User("Actor", "actor@eproc.com", "123456", UserRolesEnum.Procurement);

        PurchaseOrderService service = new(
            orderRepository,
            new FakePurchaseRequestRepository(),
            new FakePurchaseHistoryRepository(),
            new FakeSupplierRepository(),
            userRepository,
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CancelAsync(1, new PurchaseOrderActionRequest(3, "Cancel"), CancellationToken.None));
    }

    private sealed class FakePurchaseOrderRepository : IPurchaseOrderRepository
    {
        public PurchaseOrder? AddedOrder { get; private set; }
        public PurchaseOrder? OrderById { get; set; }

        public Task AddAsync(PurchaseOrder order, CancellationToken cancellationToken = default)
        {
            AddedOrder = order;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<PurchaseOrder>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<PurchaseOrder>>(OrderById is null ? [] : [OrderById]);

        public Task<PurchaseOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(OrderById);

        public void Update(PurchaseOrder order)
            => OrderById = order;
    }

    private sealed class FakePurchaseRequestRepository : IPurchaseRequestRepository
    {
        public PurchaseRequest? RequestById { get; set; }
        public bool UpdateCalled { get; private set; }

        public Task AddAsync(PurchaseRequest request, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IReadOnlyCollection<PurchaseRequest>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<PurchaseRequest>>(RequestById is null ? [] : [RequestById]);

        public Task<PurchaseRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(RequestById);

        public void Update(PurchaseRequest request)
            => UpdateCalled = true;
    }

    private sealed class FakePurchaseHistoryRepository : IPurchaseHistoryRepository
    {
        public List<PurchaseHistory> AddedHistories { get; } = [];

        public Task AddAsync(PurchaseHistory history, CancellationToken cancellationToken = default)
        {
            AddedHistories.Add(history);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<PurchaseHistory>> GetByPurchaseRequestIdAsync(int purchaseRequestId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<PurchaseHistory>>([]);
    }

    private sealed class FakeSupplierRepository : ISupplierRepository
    {
        public Supplier? SupplierById { get; set; }

        public Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(Supplier supplier, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Delete(Supplier supplier)
        {
        }

        public Task<IReadOnlyCollection<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<Supplier>>(SupplierById is null ? [] : [SupplierById]);

        public Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(SupplierById);

        public Task<(IReadOnlyCollection<Supplier> Items, int TotalCount)> GetPagedAsync(string? searchTerm, string? documentNumber, bool? isActive, int page, int pageSize, CancellationToken cancellationToken = default)
            => Task.FromResult<(IReadOnlyCollection<Supplier>, int)>((SupplierById is null ? [] : [SupplierById], SupplierById is null ? 0 : 1));

        public void Update(Supplier supplier)
            => SupplierById = supplier;
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public Dictionary<int, User> UsersById { get; } = [];

        public Task AddAsync(User user, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken) => Task.FromResult(false);

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult(UsersById.Values.FirstOrDefault(u => u.Email == email));

        public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(UsersById.TryGetValue(id, out User? user) ? user : null);
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCalls { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCalls++;
            return Task.FromResult(1);
        }
    }
}
