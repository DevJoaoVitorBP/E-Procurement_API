namespace Eprocurement.Application.Contracts
{
        /// <summary>
        /// User creation payload.
        /// </summary>
        public record CreateUserRequest(string Name, string Email);

        /// <summary>
        /// User data.
        /// </summary>
        public record UserResponse(int Id, string Name, string Email);

        /// <summary>
        /// Authentication credentials.
        /// </summary>
        public record AuthenticateUserRequest(string Email, string Password);

        /// <summary>
        /// User authentication result.
        /// </summary>
        public record AuthenticateUserResponse(int Id, string Name, string Email, string Token);

        /// <summary>
        /// User role change payload.
        /// </summary>
        public record ChangeUserRoleRequest(int UserId, string NewRole);

        /// <summary>
        /// User deactivation payload.
        /// </summary>
        public record DeactivateUserRequest(int UserId);
}
