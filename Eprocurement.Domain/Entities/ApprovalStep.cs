using Eprocurement.Domain.Enums;

namespace Eprocurement.Domain.Entities
{
    public class ApprovalStep : BaseEntity
    {
        public int PurchaseRequestId { get; private set; }
        public int ApproverUserId { get; private set; }
        public ApprovalStatusEnum Decision { get; private set; }
        public string? Comment { get; private set; }
        public DateTime? DecidedAtUtc { get; private set; }

        public ApprovalStep(int purchaseRequestId, int approverUserId)
        {
            PurchaseRequestId = purchaseRequestId;
            ApproverUserId = approverUserId;
            Decision = ApprovalStatusEnum.Pending;
        }

        public void Approve(string? comment)
        {
            Decision = ApprovalStatusEnum.Approved;
            Comment = comment;
            DecidedAtUtc = DateTime.UtcNow;
            Touch();
        }

        public void Reject(string? comment)
        {
            Decision = ApprovalStatusEnum.Rejected;
            Comment = comment;
            DecidedAtUtc = DateTime.UtcNow;
            Touch();
        }
    }
}