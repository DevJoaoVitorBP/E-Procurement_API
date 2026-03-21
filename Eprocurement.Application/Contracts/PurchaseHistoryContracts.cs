namespace Eprocurement.Application.Contracts
{
    public record PurchaseHistoryResponse(
        int Id,
        int PurchaseRequestId,
        string Action,
        string PerformedBy,
        string? Notes,
        DateTime CreatedAtUtc);
}