using Eprocurement.Application.Abstractions;
using Eprocurement.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EprocurementApi.Controllers
{
    /// <summary>
    /// Authentication endpoints and authenticated user data.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Performs login and returns a JWT token.
        /// </summary>
        /// <param name="request">User credentials.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        /// <response code="200">Login successful.</response>
        /// <response code="401">Invalid credentials.</response>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                LoginResponse response = await _authService.LoginAsync(request, cancellationToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Returns basic data of the authenticated user.
        /// </summary>
        /// <response code="200">Authenticated user data returned successfully.</response>
        /// <response code="401">Missing, invalid, or expired token.</response>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Me()
        {
            return Ok(new
            {
                UserId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value,
                Name = User.Identity?.Name,
                Email = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value,
                Role = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value
            });
        }
    }
}
