using Eprocurement.Domain.Entities;
using Xunit;

namespace Eprocurement.Tests.Domain;

public class SupplierTests
{
    [Fact]
    public void UpdateData_ShouldChangeMainFields()
    {
        Supplier supplier = new("Old Name", "123", "old@corp.com", "+55 11 1111");

        supplier.UpdateData("New Name", "new@corp.com", "+55 11 9999");

        Assert.Equal("New Name", supplier.CorporateName);
        Assert.Equal("new@corp.com", supplier.Email);
        Assert.Equal("+55 11 9999", supplier.Phone);
    }

    [Fact]
    public void DeactivateAndActivate_ShouldToggleIsActive()
    {
        Supplier supplier = new("Tech", "123", "tech@corp.com", "+55 11");

        supplier.Deactivate();
        Assert.False(supplier.IsActive);

        supplier.Activate();
        Assert.True(supplier.IsActive);
    }
}
