using Eprocurement.Domain.Enums;

namespace Eprocurement.Domain.Entities
{
    /// <summary>
    /// Payload used to register a new user.
    /// </summary>
    public class RegisterUserRequest
    {
        /// <summary>
        /// Full name.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// User email.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Plain text password used during registration.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// User access role.
        /// </summary>
        public UserRolesEnum Role { get; private set; }

        /// <summary>
        /// Indicates whether the user is active.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Creates a new user registration payload.
        /// </summary>
        /// <param name="name">Full name.</param>
        /// <param name="email">User email.</param>
        /// <param name="password">User password.</param>
        /// <param name="role">User access role.</param>
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
