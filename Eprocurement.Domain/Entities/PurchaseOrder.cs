using Eprocurement.Domain.Enums;

namespace Eprocurement.Domain.Entities
{
    public class PurchaseOrder : BaseEntity
    {
        public int PurchaseRequestId { get; private set; }
        public int SupplierId { get; private set; }
        public int CreatedByUserId { get; private set; }
        public decimal TotalAmount { get; private set; }
        public PurchaseOrderStatusEnum Status { get; private set; }

        public PurchaseOrder(int purchaseRequestId, int supplierId, int createdByUserId, decimal totalAmount)
        {
            PurchaseRequestId = purchaseRequestId;
            SupplierId = supplierId;
            CreatedByUserId = createdByUserId;
            TotalAmount = totalAmount;
            Status = PurchaseOrderStatusEnum.Created;
        }

        public void MarkAsSent() { Status = PurchaseOrderStatusEnum.SentToSupplier; Touch(); }
        public void MarkAsCompleted() { Status = PurchaseOrderStatusEnum.Completed; Touch(); }
        public void Cancel() { Status = PurchaseOrderStatusEnum.Cancelled; Touch(); }
    }
}