namespace Eprocurement.Application.Contracts
{
    public record CreatePurchaseRequestItemRequest(string Description, int Quantity, decimal UnitPrice);

    public record CreatePurchaseRequestRequest(
        int RequestedByUserId,
        string Title,
        string Justification,
        IReadOnlyCollection<CreatePurchaseRequestItemRequest> Items);

    public record PurchaseRequestResponse(
        int Id,
        int RequestedByUserId,
        string Title,
        string Justification,
        string Status,
        decimal TotalAmount);

    public record ApprovalDecisionRequest(int ApproverUserId, string? Comment);
}