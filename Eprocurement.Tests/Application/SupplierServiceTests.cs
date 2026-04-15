using Eprocurement.Application.Contracts;
using Eprocurement.Application.Services;
using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Interfaces;
using Xunit;

namespace Eprocurement.Tests.Application;

public class SupplierServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldPersistAndReturnResponse()
    {
        FakeSupplierRepository repository = new();
        FakeUnitOfWork unitOfWork = new();
        SupplierService service = new(repository, unitOfWork);

        SupplierResponse response = await service.CreateAsync(
            new CreateSupplierRequest("Tech Supplies", "123", "tech@corp.com", "+55 11 1111-1111"),
            CancellationToken.None);

        Assert.NotNull(repository.AddedSupplier);
        Assert.Equal("Tech Supplies", response.CorporateName);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task GetAllAsync_ShouldNormalizeInvalidPaging()
    {
        FakeSupplierRepository repository = new();
        repository.PagedResultItems = [new Supplier("Tech", "123", "tech@corp.com", "+55 11")];
        repository.PagedResultTotalCount = 1;

        SupplierService service = new(repository, new FakeUnitOfWork());

        PagedSupplierResponse response = await service.GetAllAsync(
            new SupplierFilterRequest { Page = 0, PageSize = 500 },
            CancellationToken.None);

        Assert.Equal(1, response.Page);
        Assert.Equal(100, response.PageSize);
        Assert.Single(response.Items);
        Assert.Equal(1, repository.LastPage);
        Assert.Equal(100, repository.LastPageSize);
    }

    [Fact]
    public async Task SetStatusAsync_ShouldDeactivateSupplier()
    {
        Supplier supplier = new("Tech", "123", "tech@corp.com", "+55 11");
        FakeSupplierRepository repository = new() { SupplierById = supplier };
        FakeUnitOfWork unitOfWork = new();
        SupplierService service = new(repository, unitOfWork);

        SupplierResponse response = await service.SetStatusAsync(1, false, CancellationToken.None);

        Assert.False(response.IsActive);
        Assert.True(repository.UpdateCalled);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task UpdateAsync_WhenSupplierNotFound_ShouldThrow()
    {
        SupplierService service = new(new FakeSupplierRepository(), new FakeUnitOfWork());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateAsync(123, new UpdateSupplierRequest("Name", "email@corp.com", "+55"), CancellationToken.None));
    }

    private sealed class FakeSupplierRepository : ISupplierRepository
    {
        public Supplier? SupplierById { get; set; }
        public Supplier? AddedSupplier { get; private set; }
        public bool UpdateCalled { get; private set; }
        public IReadOnlyCollection<Supplier> PagedResultItems { get; set; } = [];
        public int PagedResultTotalCount { get; set; }
        public int LastPage { get; private set; }
        public int LastPageSize { get; private set; }

        public Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
        {
            AddedSupplier = supplier;
            SupplierById = supplier;
            return Task.CompletedTask;
        }

        public void Delete(Supplier supplier)
        {
        }

        public Task DeleteAsync(Supplier supplier, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<IReadOnlyCollection<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(PagedResultItems);

        public Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(SupplierById);

        public Task<(IReadOnlyCollection<Supplier> Items, int TotalCount)> GetPagedAsync(string? searchTerm, string? documentNumber, bool? isActive, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            LastPage = page;
            LastPageSize = pageSize;
            return Task.FromResult((PagedResultItems, PagedResultTotalCount));
        }

        public void Update(Supplier supplier)
            => UpdateCalled = true;
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
