namespace Eprocurement.Domain.Entities
{
    public class PurchaseRequestItem : BaseEntity
    {
        public int PurchaseRequestId { get; private set; }
        public string Description { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public PurchaseRequestItem(int purchaseRequestId, string description, int quantity, decimal unitPrice)
        {
            PurchaseRequestId = purchaseRequestId;
            Description = description;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}