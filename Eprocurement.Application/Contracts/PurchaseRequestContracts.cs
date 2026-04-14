namespace Eprocurement.Application.Contracts
{
    /// <summary>
    /// Purchase request line item.
    /// </summary>
    /// <param name="Description">Item description. Example: Notebook i7 16GB RAM.</param>
    /// <param name="Quantity">Requested quantity. Example: 5.</param>
    /// <param name="UnitPrice">Unit price amount. Example: 5200.00.</param>
    public record CreatePurchaseRequestItemRequest(string Description, int Quantity, decimal UnitPrice);

    /// <summary>
    /// Purchase request creation payload.
    /// </summary>
    /// <param name="RequestedByUserId">Requester user identifier. Example: 1.</param>
    /// <param name="Title">Short title. Example: Notebook acquisition.</param>
    /// <param name="Justification">Business reason for the request.</param>
    /// <param name="Items">Requested items list.</param>
    public record CreatePurchaseRequestRequest(
        int RequestedByUserId,
        string Title,
        string Justification,
        IReadOnlyCollection<CreatePurchaseRequestItemRequest> Items);

    /// <summary>
    /// Purchase request representation.
    /// </summary>
    public record PurchaseRequestResponse(
        int Id,
        int RequestedByUserId,
        string Title,
        string Justification,
        string Status,
        decimal TotalAmount);

    /// <summary>
    /// Purchase request approval/rejection decision payload.
    /// </summary>
    /// <param name="ApproverUserId">Approver user identifier. Example: 2.</param>
    /// <param name="Comment">Optional decision comment.</param>
    public record ApprovalDecisionRequest(int ApproverUserId, string? Comment);

    /// <summary>
    /// Operational action payload for a purchase request.
    /// </summary>
    /// <param name="PerformedByUserId">User identifier performing the action. Example: 3.</param>
    /// <param name="Comment">Optional action comment.</param>
    public record PurchaseRequestActionRequest(int PerformedByUserId, string? Comment);
}