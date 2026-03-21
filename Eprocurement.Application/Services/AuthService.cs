using Eprocurement.Application.Abstractions;
using Eprocurement.Application.Contracts;
using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Interfaces;

namespace Eprocurement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            User? user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user is null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("Credenciais inválidas.");
            }

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Credenciais inválidas.");
            }

            string token = _tokenService.GenerateToken(user);

            return new LoginResponse(
                token,
                user.Name,
                user.Email,
                user.Role.ToString());
        }
    }
}