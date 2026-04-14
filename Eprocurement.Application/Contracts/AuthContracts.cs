namespace Eprocurement.Application.Contracts
{
    /// <summary>
    /// User authentication payload.
    /// </summary>
    /// <param name="Email">Registered user email. Example: admin@eproc.com.</param>
    /// <param name="Password">User password. Example: 123456.</param>
    public record LoginRequest(string Email, string Password);

    /// <summary>
    /// Authentication result with JWT token and user data.
    /// </summary>
    /// <param name="Token">Access JWT token.</param>
    /// <param name="Name">Authenticated user name.</param>
    /// <param name="Email">Authenticated user email.</param>
    /// <param name="Role">Authenticated user role.</param>
    public record LoginResponse(string Token, string Name, string Email, string Role);
}