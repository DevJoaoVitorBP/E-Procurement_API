using Eprocurement.Domain.Enums;

namespace Eprocurement.Domain.Entities
{
    public class RegisterUserRequest
    {
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; }
        public string Password { get; private set; }
        public UserRolesEnum Role { get; private set; }
        public bool IsActive { get; private set; }

        public RegisterUserRequest(string name, string email, string password, UserRolesEnum role)
        {
            Name = name;
            Email = email;
            Password = password;
            Role = role;
            IsActive = true;
        }
    }
}
