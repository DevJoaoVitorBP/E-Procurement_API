using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Enums;
using Xunit;

namespace Eprocurement.Tests.Domain;

public class PurchaseRequestTests
{
    [Fact]
    public void Constructor_WithInvalidRequester_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PurchaseRequest(0, "Title", "Justification"));
    }

    [Fact]
    public void AddItem_WithInvalidQuantity_ShouldThrow()
    {
        PurchaseRequest request = new(1, "Title", "Justification");

        Assert.Throws<ArgumentOutOfRangeException>(() => request.AddItem("Notebook", 0, 100));
    }

    [Fact]
    public void RejectByManager_WhenNotPending_ShouldThrow()
    {
        PurchaseRequest request = new(1, "Title", "Justification");
        request.ApproveByManager();

        Assert.Throws<InvalidOperationException>(() => request.RejectByManager());
    }

    [Fact]
    public void MarkAsOrdered_WhenNotInProcurement_ShouldThrow()
    {
        PurchaseRequest request = new(1, "Title", "Justification");

        Assert.Throws<InvalidOperationException>(() => request.MarkAsOrdered());
    }

    [Fact]
    public void ValidFlow_ShouldMoveToOrdered()
    {
        PurchaseRequest request = new(1, "Title", "Justification");

        request.AddItem("Notebook", 2, 5000);
        request.ApproveByManager();
        request.MoveToProcurement();
        request.MarkAsOrdered();

        Assert.Equal(PurchaseRequestStatusEnum.Ordered, request.Status);
        Assert.Equal(10000, request.TotalAmount);
    }
}
