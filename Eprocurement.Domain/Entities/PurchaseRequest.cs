using Eprocurement.Domain.Enums;

namespace Eprocurement.Domain.Entities
{
    public class PurchaseRequest : BaseEntity
    {
        public int RequestedByUserId { get; private set; }
        public string Title { get; private set; }
        public string Justification { get; private set; }
        public PurchaseRequestStatusEnum Status { get; private set; }

        public List<PurchaseRequestItem> Items { get; private set; } = new();

        public PurchaseRequest(int requestedByUserId, string title, string justification)
        {
            RequestedByUserId = requestedByUserId;
            Title = title;
            Justification = justification;
            Status = PurchaseRequestStatusEnum.PendingManagerApproval;
        }

        public decimal TotalAmount => Items.Sum(i => i.Quantity * i.UnitPrice);

        public void AddItem(string description, int quantity, decimal unitPrice)
        {
            Items.Add(new PurchaseRequestItem(Id, description, quantity, unitPrice));
            Touch();
        }

        public void ApproveByManager()
        {
            Status = PurchaseRequestStatusEnum.ApprovedByManager;
            Touch();
        }

        public void RejectByManager()
        {
            Status = PurchaseRequestStatusEnum.RejectedByManager;
            Touch();
        }

        public void MoveToProcurement()
        {
            Status = PurchaseRequestStatusEnum.InProcurement;
            Touch();
        }

        public void MarkAsOrdered()
        {
            Status = PurchaseRequestStatusEnum.Ordered;
            Touch();
        }
    }
}