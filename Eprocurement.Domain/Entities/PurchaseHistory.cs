namespace Eprocurement.Domain.Entities
{
    public class PurchaseHistory : BaseEntity
    {
        public int PurchaseRequestId { get; private set; }
        public string Action { get; private set; }
        public string PerformedBy { get; private set; }
        public string? Notes { get; private set; }

        public PurchaseHistory(int purchaseRequestId, string action, string performedBy, string? notes = null)
        {
            PurchaseRequestId = purchaseRequestId;
            Action = action;
            PerformedBy = performedBy;
            Notes = notes;
        }
    }
}