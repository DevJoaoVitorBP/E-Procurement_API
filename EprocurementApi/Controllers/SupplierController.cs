using Eprocurement.Application.Abstractions;
using Eprocurement.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EprocurementApi.Controllers
{
    /// <summary>
    /// Supplier management endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(ISupplierService supplierService, ILogger<SupplierController> logger)
        {
            _supplierService = supplierService;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new supplier.
        /// </summary>
        /// <param name="request">Supplier creation payload.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpPost("register")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(SupplierResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] CreateSupplierRequest request, CancellationToken cancellationToken)
        {
            try
            {
                SupplierResponse supplier = await _supplierService.CreateAsync(request, cancellationToken);

                return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Supplier registration failed for email: {Email}", request.Email);
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Returns suppliers using filters and pagination.
        /// </summary>
        /// <param name="filter">Filter and pagination parameters.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(PagedSupplierResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] SupplierFilterRequest filter, CancellationToken cancellationToken)
        {
            PagedSupplierResponse suppliers = await _supplierService.GetAllAsync(filter, cancellationToken);
            return Ok(suppliers);
        }

        /// <summary>
        /// Gets a supplier by identifier.
        /// </summary>
        /// <param name="id">Supplier identifier.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(SupplierResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            try
            {
                SupplierResponse supplier = await _supplierService.GetByIdAsync(id, cancellationToken);
                return Ok(supplier);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Supplier not found. Id: {SupplierId}", id);
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates supplier data.
        /// </summary>
        /// <param name="id">Supplier identifier.</param>
        /// <param name="request">Supplier update payload.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(SupplierResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSupplierRequest request, CancellationToken cancellationToken)
        {
            try
            {
                SupplierResponse supplier = await _supplierService.UpdateAsync(id, request, cancellationToken);
                return Ok(supplier);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Supplier update failed. Id: {SupplierId}", id);
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Changes supplier status (active/inactive).
        /// </summary>
        /// <param name="id">Supplier identifier.</param>
        /// <param name="request">New status payload.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(SupplierResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeSupplierStatusRequest request, CancellationToken cancellationToken)
        {
            try
            {
                SupplierResponse supplier = await _supplierService.SetStatusAsync(id, request.IsActive, cancellationToken);
                return Ok(supplier);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Supplier status change failed. Id: {SupplierId}", id);
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a supplier.
        /// </summary>
        /// <param name="id">Supplier identifier.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _supplierService.DeleteAsync(id, cancellationToken);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Supplier delete failed. Id: {SupplierId}", id);
                return NotFound(new { message = ex.Message });
            }
        }
    }
}