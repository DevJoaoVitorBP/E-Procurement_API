namespace Eprocurement.Application.Contracts
{
    /// <summary>
    /// Supplier creation payload.
    /// </summary>
    /// <param name="CorporateName">Legal entity name. Example: Tech Supplies LTDA.</param>
    /// <param name="DocumentNumber">Tax identifier. Example: 12345678000199.</param>
    /// <param name="Email">Supplier contact email.</param>
    /// <param name="Phone">Supplier contact phone number.</param>
    public record CreateSupplierRequest(string CorporateName, string DocumentNumber, string Email, string Phone);

    /// <summary>
    /// Supplier update payload.
    /// </summary>
    /// <param name="CorporateName">Updated legal entity name.</param>
    /// <param name="Email">Updated contact email.</param>
    /// <param name="Phone">Updated contact phone number.</param>
    public record UpdateSupplierRequest(string CorporateName, string Email, string Phone);

    /// <summary>
    /// Supplier status change payload.
    /// </summary>
    /// <param name="IsActive">True to activate, false to deactivate.</param>
    public record ChangeSupplierStatusRequest(bool IsActive);

    /// <summary>
    /// Supplier paged search filters.
    /// </summary>
    public sealed class SupplierFilterRequest
    {
        /// <summary>
        /// Free text term used for supplier search.
        /// </summary>
        public string? SearchTerm { get; init; }

        /// <summary>
        /// Supplier document number (tax identifier).
        /// </summary>
        public string? DocumentNumber { get; init; }

        /// <summary>
        /// Supplier active status.
        /// </summary>
        public bool? IsActive { get; init; }

        /// <summary>
        /// Current page number (starts at 1).
        /// </summary>
        public int Page { get; init; } = 1;

        /// <summary>
        /// Number of records per page.
        /// </summary>
        public int PageSize { get; init; } = 10;
    }

    /// <summary>
    /// Paged supplier query response.
    /// </summary>
    public record PagedSupplierResponse(
        IReadOnlyCollection<SupplierResponse> Items,
        int Page,
        int PageSize,
        int TotalCount,
        int TotalPages);

    /// <summary>
    /// Supplier data.
    /// </summary>
    public record SupplierResponse(int Id, string CorporateName, string DocumentNumber, string Email, string Phone, bool IsActive);
}