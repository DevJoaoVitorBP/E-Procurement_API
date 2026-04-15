using Eprocurement.Application.Services;
using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Enums;
using Eprocurement.Domain.Interfaces;
using Xunit;

namespace Eprocurement.Tests.Application;

public class UserServiceTests
{
    [Fact]
    public async Task RegisterAsync_WhenEmailExists_ShouldThrow()
    {
        FakeUserRepository repository = new() { EmailExistsResult = true };
        UserService service = new(repository, new FakeUnitOfWork());

        RegisterUserRequest request = new("Admin", "admin@eproc.com", "123456", UserRolesEnum.Admin);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegisterAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task RegisterAsync_WhenValid_ShouldAddAndSave()
    {
        FakeUserRepository repository = new();
        FakeUnitOfWork unitOfWork = new();
        UserService service = new(repository, unitOfWork);

        RegisterUserRequest request = new("Admin", "admin@eproc.com", "123456", UserRolesEnum.Admin);

        await service.RegisterAsync(request, CancellationToken.None);

        Assert.NotNull(repository.AddedUser);
        Assert.Equal("Admin", repository.AddedUser!.Name);
        Assert.Equal("admin@eproc.com", repository.AddedUser.Email);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public bool EmailExistsResult { get; set; }
        public User? AddedUser { get; private set; }

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            AddedUser = user;
            return Task.CompletedTask;
        }

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
            => Task.FromResult(EmailExistsResult);

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult<User?>(null);

        public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult<User?>(null);
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
