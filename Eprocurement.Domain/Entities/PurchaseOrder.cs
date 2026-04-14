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
            if (purchaseRequestId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(purchaseRequestId), "Purchase request id must be greater than zero.");
            }

            if (supplierId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(supplierId), "Supplier id must be greater than zero.");
            }

            if (createdByUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(createdByUserId), "Created by user id must be greater than zero.");
            }

            if (totalAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalAmount), "Total amount must be greater than zero.");
            }

            PurchaseRequestId = purchaseRequestId;
            SupplierId = supplierId;
            CreatedByUserId = createdByUserId;
            TotalAmount = totalAmount;
            Status = PurchaseOrderStatusEnum.Created;
        }

        public void MarkAsSent()
        {
            if (Status != PurchaseOrderStatusEnum.Created)
            {
                throw new InvalidOperationException("Only created orders can be marked as sent.");
            }

            Status = PurchaseOrderStatusEnum.SentToSupplier;
            Touch();
        }

        public void MarkAsCompleted()
        {
            if (Status != PurchaseOrderStatusEnum.SentToSupplier)
            {
                throw new InvalidOperationException("Only sent orders can be completed.");
            }

            Status = PurchaseOrderStatusEnum.Completed;
            Touch();
        }

        public void Cancel()
        {
            if (Status is PurchaseOrderStatusEnum.Completed or PurchaseOrderStatusEnum.Cancelled)
            {
                throw new InvalidOperationException("Completed or cancelled orders cannot be cancelled again.");
            }

            Status = PurchaseOrderStatusEnum.Cancelled;
            Touch();
        }
    }
}