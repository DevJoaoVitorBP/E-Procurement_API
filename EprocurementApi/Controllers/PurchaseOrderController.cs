using Eprocurement.Application.Abstractions;
using Eprocurement.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EprocurementApi.Controllers
{
    /// <summary>
    /// Purchase order management endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly ILogger<PurchaseOrderController> _logger;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService, ILogger<PurchaseOrderController> logger)
        {
            _purchaseOrderService = purchaseOrderService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new purchase order.
        /// </summary>
        /// <param name="request">Purchase order creation payload.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpPost]
        [Authorize(Roles = "Procurement,Admin")]
        [ProducesResponseType(typeof(PurchaseOrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                PurchaseOrderResponse response = await _purchaseOrderService.CreateAsync(request, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Purchase order creation failed due to missing dependency. Request: {@Request}", request);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid purchase order creation. Request: {@Request}", request);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Returns all purchase orders.
        /// </summary>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IReadOnlyCollection<PurchaseOrderResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            IReadOnlyCollection<PurchaseOrderResponse> responses = await _purchaseOrderService.GetAllAsync(cancellationToken);
            return Ok(responses);
        }

        /// <summary>
        /// Gets a purchase order by identifier.
        /// </summary>
        /// <param name="id">Purchase order identifier.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(PurchaseOrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            PurchaseOrderResponse? response = await _purchaseOrderService.GetByIdAsync(id, cancellationToken);
            if (response is null)
            {
                return NotFound(new { message = "Purchase order not found." });
            }

            return Ok(response);
        }

        /// <summary>
        /// Marks a purchase order as sent to the supplier.
        /// </summary>
        /// <param name="id">Purchase order identifier.</param>
        /// <param name="request">Action payload.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpPatch("{id:int}/send")]
        [Authorize(Roles = "Procurement,Admin")]
        [ProducesResponseType(typeof(PurchaseOrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkAsSent(int id, [FromBody] PurchaseOrderActionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                PurchaseOrderResponse response = await _purchaseOrderService.MarkAsSentAsync(id, request, cancellationToken);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Purchase order not found for send operation. Id: {PurchaseOrderId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid send operation for purchase order. Id: {PurchaseOrderId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Marks a purchase order as completed.
        /// </summary>
        /// <param name="id">Purchase order identifier.</param>
        /// <param name="request">Action payload.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpPatch("{id:int}/complete")]
        [Authorize(Roles = "Procurement,Admin")]
        [ProducesResponseType(typeof(PurchaseOrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkAsCompleted(int id, [FromBody] PurchaseOrderActionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                PurchaseOrderResponse response = await _purchaseOrderService.MarkAsCompletedAsync(id, request, cancellationToken);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Purchase order not found for complete operation. Id: {PurchaseOrderId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid complete operation for purchase order. Id: {PurchaseOrderId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cancels a purchase order.
        /// </summary>
        /// <param name="id">Purchase order identifier.</param>
        /// <param name="request">Action payload.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpPatch("{id:int}/cancel")]
        [Authorize(Roles = "Procurement,Admin")]
        [ProducesResponseType(typeof(PurchaseOrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel(int id, [FromBody] PurchaseOrderActionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                PurchaseOrderResponse response = await _purchaseOrderService.CancelAsync(id, request, cancellationToken);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Purchase order not found for cancel operation. Id: {PurchaseOrderId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid cancel operation for purchase order. Id: {PurchaseOrderId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
