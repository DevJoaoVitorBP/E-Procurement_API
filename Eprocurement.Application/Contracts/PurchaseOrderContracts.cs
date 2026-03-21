namespace Eprocurement.Application.Contracts
{
    public record CreatePurchaseOrderRequest(int PurchaseRequestId, int SupplierId, int CreatedByUserId);

    public record PurchaseOrderResponse(
        int Id,
        int PurchaseRequestId,
        int SupplierId,
        int CreatedByUserId,
        decimal TotalAmount,
        string Status);
}