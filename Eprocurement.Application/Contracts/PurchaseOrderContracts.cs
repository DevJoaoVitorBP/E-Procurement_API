namespace Eprocurement.Application.Contracts
{
    /// <summary>
    /// Purchase order creation payload.
    /// </summary>
    /// <param name="PurchaseRequestId">Approved purchase request identifier. Example: 1.</param>
    /// <param name="SupplierId">Selected supplier identifier. Example: 1.</param>
    /// <param name="CreatedByUserId">User identifier that creates the order. Example: 3.</param>
    public record CreatePurchaseOrderRequest(int PurchaseRequestId, int SupplierId, int CreatedByUserId);

    /// <summary>
    /// Purchase order representation.
    /// </summary>
    public record PurchaseOrderResponse(
        int Id,
        int PurchaseRequestId,
        int SupplierId,
        int CreatedByUserId,
        decimal TotalAmount,
        string Status);

    /// <summary>
    /// Operational action payload for a purchase order.
    /// </summary>
    /// <param name="PerformedByUserId">User identifier performing the action. Example: 3.</param>
    /// <param name="Comment">Optional action comment.</param>
    public record PurchaseOrderActionRequest(int PerformedByUserId, string? Comment);
}