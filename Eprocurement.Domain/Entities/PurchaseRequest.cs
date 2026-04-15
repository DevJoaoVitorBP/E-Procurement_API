using Eprocurement.Domain.Enums;

namespace Eprocurement.Domain.Entities
{
    /// <summary>
    /// Represents a purchase request and its workflow state.
    /// </summary>
    public class PurchaseRequest : BaseEntity
    {
        /// <summary>
        /// Requester user identifier.
        /// </summary>
        public int RequestedByUserId { get; private set; }

        /// <summary>
        /// Short title for the request.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Business justification for the request.
        /// </summary>
        public string Justification { get; private set; }

        /// <summary>
        /// Current workflow status.
        /// </summary>
        public PurchaseRequestStatusEnum Status { get; private set; }

        /// <summary>
        /// Requested line items.
        /// </summary>
        public List<PurchaseRequestItem> Items { get; private set; } = new();

        /// <summary>
        /// Initializes a new purchase request.
        /// </summary>
        /// <param name="requestedByUserId">Requester user identifier.</param>
        /// <param name="title">Request title.</param>
        /// <param name="justification">Request justification.</param>
        public PurchaseRequest(int requestedByUserId, string title, string justification)
        {
            if (requestedByUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requestedByUserId), "Requested by user id must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title is required.", nameof(title));
            }

            if (string.IsNullOrWhiteSpace(justification))
            {
                throw new ArgumentException("Justification is required.", nameof(justification));
            }

            RequestedByUserId = requestedByUserId;
            Title = title;
            Justification = justification;
            Status = PurchaseRequestStatusEnum.PendingManagerApproval;
        }

        /// <summary>
        /// Gets the total amount computed from all items.
        /// </summary>
        public decimal TotalAmount => Items.Sum(i => i.Quantity * i.UnitPrice);

        /// <summary>
        /// Adds a line item to the request.
        /// </summary>
        /// <param name="description">Item description.</param>
        /// <param name="quantity">Requested quantity.</param>
        /// <param name="unitPrice">Item unit price.</param>
        public void AddItem(string description, int quantity, decimal unitPrice)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Item description is required.", nameof(description));
            }

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
            }

            if (unitPrice <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price must be greater than zero.");
            }

            Items.Add(new PurchaseRequestItem(Id, description, quantity, unitPrice));
            Touch();
        }

        /// <summary>
        /// Approves the request by manager.
        /// </summary>
        public void ApproveByManager()
        {
            if (Status != PurchaseRequestStatusEnum.PendingManagerApproval)
            {
                throw new InvalidOperationException("Only requests pending manager approval can be approved.");
            }

            Status = PurchaseRequestStatusEnum.ApprovedByManager;
            Touch();
        }

        /// <summary>
        /// Rejects the request by manager.
        /// </summary>
        public void RejectByManager()
        {
            if (Status != PurchaseRequestStatusEnum.PendingManagerApproval)
            {
                throw new InvalidOperationException("Only requests pending manager approval can be rejected.");
            }

            Status = PurchaseRequestStatusEnum.RejectedByManager;
            Touch();
        }

        /// <summary>
        /// Moves the request to procurement workflow.
        /// </summary>
        public void MoveToProcurement()
        {
            if (Status != PurchaseRequestStatusEnum.ApprovedByManager)
            {
                throw new InvalidOperationException("Only approved requests can be moved to procurement.");
            }

            Status = PurchaseRequestStatusEnum.InProcurement;
            Touch();
        }

        /// <summary>
        /// Marks the request as ordered.
        /// </summary>
        public void MarkAsOrdered()
        {
            if (Status != PurchaseRequestStatusEnum.InProcurement)
            {
                throw new InvalidOperationException("Only requests in procurement can be marked as ordered.");
            }

            Status = PurchaseRequestStatusEnum.Ordered;
            Touch();
        }
    }
}