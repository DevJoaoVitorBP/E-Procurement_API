using Eprocurement.Application.Contracts;
using Eprocurement.Application.Services;
using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Enums;
using Eprocurement.Domain.Interfaces;
using Xunit;

namespace Eprocurement.Tests.Application;

public class PurchaseRequestServiceTests
{
    [Fact]
    public async Task MoveToProcurement_WhenRequestIsNotApproved_ShouldThrowInvalidOperation()
    {
        PurchaseRequest request = new(1, "Title", "Justification");

        FakePurchaseRequestRepository purchaseRequestRepository = new() { RequestById = request };
        FakePurchaseHistoryRepository purchaseHistoryRepository = new();
        FakeUserRepository userRepository = new();
        userRepository.UsersById[3] = new User("Actor", "actor@eproc.com", "123456", UserRolesEnum.Procurement);
        FakeUnitOfWork unitOfWork = new();

        PurchaseRequestService service = new(purchaseRequestRepository, purchaseHistoryRepository, userRepository, unitOfWork);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.MoveToProcurementAsync(1, new PurchaseRequestActionRequest(3, "Moving"), CancellationToken.None));
    }

    [Fact]
    public async Task MoveToProcurement_WhenRequestIsApproved_ShouldUpdateAndSave()
    {
        PurchaseRequest request = new(1, "Title", "Justification");
        request.ApproveByManager();

        FakePurchaseRequestRepository purchaseRequestRepository = new() { RequestById = request };
        FakePurchaseHistoryRepository purchaseHistoryRepository = new();
        FakeUserRepository userRepository = new();
        userRepository.UsersById[3] = new User("Actor", "actor@eproc.com", "123456", UserRolesEnum.Procurement);
        FakeUnitOfWork unitOfWork = new();

        PurchaseRequestService service = new(purchaseRequestRepository, purchaseHistoryRepository, userRepository, unitOfWork);

        PurchaseRequestResponse response = await service.MoveToProcurementAsync(
            1,
            new PurchaseRequestActionRequest(3, "Moved"),
            CancellationToken.None);

        Assert.Equal(PurchaseRequestStatusEnum.InProcurement.ToString(), response.Status);
        Assert.True(purchaseRequestRepository.UpdateCalled);
        Assert.Single(purchaseHistoryRepository.AddedHistories);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistRequestAndCreateHistory()
    {
        FakePurchaseRequestRepository purchaseRequestRepository = new();
        FakePurchaseHistoryRepository purchaseHistoryRepository = new();
        FakeUserRepository userRepository = new();
        userRepository.UsersById[1] = new User("Requester", "requester@eproc.com", "123456", UserRolesEnum.Employee);
        FakeUnitOfWork unitOfWork = new();

        PurchaseRequestService service = new(purchaseRequestRepository, purchaseHistoryRepository, userRepository, unitOfWork);

        CreatePurchaseRequestRequest createRequest = new(
            1,
            "Notebook acquisition",
            "Hardware refresh",
            [new CreatePurchaseRequestItemRequest("Notebook", 2, 5000)]);

        PurchaseRequestResponse response = await service.CreateAsync(createRequest, CancellationToken.None);

        Assert.Equal("Notebook acquisition", response.Title);
        Assert.NotNull(purchaseRequestRepository.AddedRequest);
        Assert.Single(purchaseHistoryRepository.AddedHistories);
        Assert.Equal("PurchaseRequestCreated", purchaseHistoryRepository.AddedHistories[0].Action);
        Assert.Equal(2, unitOfWork.SaveChangesCalls);
    }

    private sealed class FakePurchaseRequestRepository : IPurchaseRequestRepository
    {
        public PurchaseRequest? RequestById { get; set; }
        public PurchaseRequest? AddedRequest { get; private set; }
        public bool UpdateCalled { get; private set; }

        public Task AddAsync(PurchaseRequest request, CancellationToken cancellationToken = default)
        {
            AddedRequest = request;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<PurchaseRequest>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<PurchaseRequest>>(Array.Empty<PurchaseRequest>());

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
            => Task.FromResult<IReadOnlyCollection<PurchaseHistory>>(Array.Empty<PurchaseHistory>());
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public Dictionary<int, User> UsersById { get; } = [];

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
            => Task.FromResult(false);

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
