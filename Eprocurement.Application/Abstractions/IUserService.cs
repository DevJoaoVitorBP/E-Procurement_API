using Eprocurement.Application.Contracts;
using Eprocurement.Domain.Entities;

namespace Eprocurement.Application.Abstractions
{
    /// <summary>
    /// User application service contract.
    /// </summary>
    public interface IUserService
    {
        // Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
        // Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        // Task<UserResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">User registration payload.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
       Task RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken);
    }
}
