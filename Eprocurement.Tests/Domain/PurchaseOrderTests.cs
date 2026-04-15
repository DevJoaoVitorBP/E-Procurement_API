using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Enums;
using Xunit;

namespace Eprocurement.Tests.Domain;

public class PurchaseOrderTests
{
    [Fact]
    public void Constructor_WithInvalidTotalAmount_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PurchaseOrder(1, 1, 1, 0));
    }

    [Fact]
    public void MarkAsCompleted_WhenStatusIsNotSent_ShouldThrow()
    {
        PurchaseOrder order = new(1, 1, 1, 100);

        Assert.Throws<InvalidOperationException>(() => order.MarkAsCompleted());
    }

    [Fact]
    public void ValidFlow_ShouldTransitionStatuses()
    {
        PurchaseOrder order = new(1, 1, 1, 100);

        order.MarkAsSent();
        order.MarkAsCompleted();

        Assert.Equal(PurchaseOrderStatusEnum.Completed, order.Status);
    }
}
