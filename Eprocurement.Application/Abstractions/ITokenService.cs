using Eprocurement.Domain.Entities;

namespace Eprocurement.Application.Abstractions
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}