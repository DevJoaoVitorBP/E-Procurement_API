using Eprocurement.Application.Contracts;

namespace Eprocurement.Application.Abstractions
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }
}