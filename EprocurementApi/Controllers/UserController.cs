using Eprocurement.Application.Abstractions;
using Eprocurement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EprocurementApi.Controllers
{
    /// <summary>
    /// User management endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">User registration payload.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        /// <response code="201">User registered successfully.</response>
        /// <response code="409">A user with the provided email already exists.</response>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _userService.RegisterAsync(request, cancellationToken);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "User registration failed for email: {Email}", request.Email);
                return Conflict(new { message = ex.Message });
            }
        }
    }
}