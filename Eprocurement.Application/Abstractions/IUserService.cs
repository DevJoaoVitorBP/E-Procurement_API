using Eprocurement.Application.Contracts;
using Eprocurement.Domain.Entities;

namespace Eprocurement.Application.Abstractions
{
    public interface IUserService
    {
       // Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
         //Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
         ///Task<UserResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
       Task RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken);
    }
}
