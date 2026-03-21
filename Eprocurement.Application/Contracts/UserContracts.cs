namespace Eprocurement.Application.Contracts
{
        public record CreateUserRequest(string Name, string Email);
        public record UserResponse(int Id, string Name, string Email);
        public record AuthenticateUserRequest(string Email, string Password);
        public record AuthenticateUserResponse(int Id, string Name, string Email, string Token);
        public record ChangeUserRoleRequest(int UserId, string NewRole);
        public record DeactivateUserRequest(int UserId);
}
