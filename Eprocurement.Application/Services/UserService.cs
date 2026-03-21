using Eprocurement.Application.Abstractions;
using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Interfaces;

namespace Eprocurement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
        {
            if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
            {
                throw new InvalidOperationException("Email already exists.");
            }

            Domain.Entities.User user = new Domain.Entities.User(
                request.Name,
                request.Email,
                request.Password,
                request.Role);

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
