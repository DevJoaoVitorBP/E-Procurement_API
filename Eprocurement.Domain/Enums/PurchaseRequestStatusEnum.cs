using System;
using System.Collections.Generic;
using System.Text;

namespace Eprocurement.Domain.Enums
{
    public enum PurchaseRequestStatusEnum
    {
        Draft = 1,
        PendingManagerApproval = 2,
        ApprovedByManager = 3,
        RejectedByManager = 4,
        InProcurement = 5,
        Ordered = 6,
        Cancelled = 7
    }
}

