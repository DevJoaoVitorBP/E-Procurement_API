using Eprocurement.Application.Abstractions;
using Eprocurement.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EprocurementApi.Controllers
{
    /// <summary>
    /// Purchase request management endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseRequestController : ControllerBase
    {
        private readonly IPurchaseRequestService _purchaseRequestService;
        private readonly ILogger<PurchaseRequestController> _logger;

        public PurchaseRequestController(IPurchaseRequestService purchaseRequestService, ILogger<PurchaseRequestController> logger)
        {
            _purchaseRequestService = purchaseRequestService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new purchase request.
        /// </summary>
        /// <param name="request">Purchase request creation payload.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpPost]
        [Authorize(Roles = "Employee,Manager,Admin")]
        [ProducesResponseType(typeof(PurchaseRequestResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseRequestRequest request, CancellationToken cancellationToken)
        {
            try
            {
                PurchaseRequestResponse response = await _purchaseRequestService.CreateAsync(request, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Purchase request creation failed for requester: {RequestedByUserId}", request.RequestedByUserId);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Returns all purchase requests.
        /// </summary>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IReadOnlyCollection<PurchaseRequestResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            IReadOnlyCollection<PurchaseRequestResponse> responses = await _purchaseRequestService.GetAllAsync(cancellationToken);
            return Ok(responses);
        }

        /// <summary>
        /// Gets a purchase request by identifier.
        /// </summary>
        /// <param name="id">Purchase request identifier.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(PurchaseRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            PurchaseRequestResponse? response = await _purchaseRequestService.GetByIdAsync(id, cancellationToken);
            if (response is null)
            {
                return NotFound(new { message = "Purchase request not found." });
            }

            return Ok(response);
        }

        /// <summary>
        /// Approves a purchase request.
        /// </summary>
        /// <param name="id">Purchase request identifier.</param>
        /// <param name="request">Approval payload.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpPost("{id:int}/approve")]
        [Authorize(Roles = "Manager,Admin")]
        [ProducesResponseType(typeof(PurchaseRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Approve(int id, [FromBody] ApprovalDecisionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                PurchaseRequestResponse response = await _purchaseRequestService.ApproveAsync(id, request, cancellationToken);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Purchase request not found for approval. Id: {PurchaseRequestId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid approval operation. Id: {PurchaseRequestId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Rejects a purchase request.
        /// </summary>
        /// <param name="id">Purchase request identifier.</param>
        /// <param name="request">Rejection payload.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpPost("{id:int}/reject")]
        [Authorize(Roles = "Manager,Admin")]
        [ProducesResponseType(typeof(PurchaseRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Reject(int id, [FromBody] ApprovalDecisionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                PurchaseRequestResponse response = await _purchaseRequestService.RejectAsync(id, request, cancellationToken);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Purchase request not found for rejection. Id: {PurchaseRequestId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid rejection operation. Id: {PurchaseRequestId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Moves the request to the procurement flow.
        /// </summary>
        /// <param name="id">Purchase request identifier.</param>
        /// <param name="request">Action payload.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpPost("{id:int}/move-to-procurement")]
        [Authorize(Roles = "Procurement,Manager,Admin")]
        [ProducesResponseType(typeof(PurchaseRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MoveToProcurement(int id, [FromBody] PurchaseRequestActionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                PurchaseRequestResponse response = await _purchaseRequestService.MoveToProcurementAsync(id, request, cancellationToken);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Purchase request not found for move to procurement. Id: {PurchaseRequestId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid move to procurement operation. Id: {PurchaseRequestId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Returns the purchase request action history.
        /// </summary>
        /// <param name="id">Purchase request identifier.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpGet("{id:int}/history")]
        [Authorize]
        [ProducesResponseType(typeof(IReadOnlyCollection<PurchaseHistoryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistory(int id, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<PurchaseHistoryResponse> history = await _purchaseRequestService.GetHistoryAsync(id, cancellationToken);
            return Ok(history);
        }
    }
}
