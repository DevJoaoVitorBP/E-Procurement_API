using Eprocurement.Application.Abstractions;
using Eprocurement.Application.Contracts;
using Eprocurement.Application.Services;
using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Enums;
using Eprocurement.Domain.Interfaces;
using Xunit;

namespace Eprocurement.Tests.Application;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ShouldThrowUnauthorized()
    {
        AuthService service = new(new FakeUserRepository(), new FakeTokenService(), new FakePasswordHasher(true));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.LoginAsync(new LoginRequest("none@eproc.com", "123456"), CancellationToken.None));
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsInvalid_ShouldThrowUnauthorized()
    {
        FakeUserRepository userRepository = new();
        userRepository.UserByEmail = new User("Admin", "admin@eproc.com", "123456", UserRolesEnum.Admin);

        AuthService service = new(userRepository, new FakeTokenService(), new FakePasswordHasher(false));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.LoginAsync(new LoginRequest("admin@eproc.com", "wrong"), CancellationToken.None));
    }

    [Fact]
    public async Task LoginAsync_WhenValid_ShouldReturnTokenResponse()
    {
        FakeUserRepository userRepository = new();
        userRepository.UserByEmail = new User("Admin", "admin@eproc.com", "123456", UserRolesEnum.Admin);

        AuthService service = new(userRepository, new FakeTokenService(), new FakePasswordHasher(true));

        LoginResponse response = await service.LoginAsync(new LoginRequest("admin@eproc.com", "123456"), CancellationToken.None);

        Assert.Equal("token-123", response.Token);
        Assert.Equal("Admin", response.Name);
        Assert.Equal("admin@eproc.com", response.Email);
        Assert.Equal("Admin", response.Role);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public User? UserByEmail { get; set; }

        public Task AddAsync(User user, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken) => Task.FromResult(false);

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult(UserByEmail?.Email == email ? UserByEmail : null);

        public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult<User?>(null);
    }

    private sealed class FakeTokenService : ITokenService
    {
        public string GenerateToken(User user) => "token-123";
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        private readonly bool _verifyResult;

        public FakePasswordHasher(bool verifyResult)
        {
            _verifyResult = verifyResult;
        }

        public string Hash(string password) => "hashed";

        public bool Verify(string password, string passwordHash) => _verifyResult;
    }
}
