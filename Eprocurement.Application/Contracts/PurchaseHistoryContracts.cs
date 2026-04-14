namespace Eprocurement.Application.Contracts
{
    /// <summary>
    /// Historical action entry for a purchase request.
    /// </summary>
    public record PurchaseHistoryResponse(
        int Id,
        int PurchaseRequestId,
        string Action,
        string PerformedBy,
        string? Notes,
        DateTime CreatedAtUtc);
}