using Eprocurement.Application.Contracts;
using Eprocurement.Application.Validators;
using Xunit;

namespace Eprocurement.Tests.Application;

public class ValidatorsTests
{
    [Fact]
    public void LoginRequestValidator_WithInvalidEmail_ShouldFail()
    {
        LoginRequestValidator validator = new();
        LoginRequest model = new("invalid", "123456");

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateSupplierRequestValidator_WithMissingCorporateName_ShouldFail()
    {
        CreateSupplierRequestValidator validator = new();
        CreateSupplierRequest model = new("", "123", "tech@corp.com", "+55 11");

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreatePurchaseOrderRequestValidator_WithInvalidIds_ShouldFail()
    {
        CreatePurchaseOrderRequestValidator validator = new();
        CreatePurchaseOrderRequest model = new(0, 0, 0);

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreatePurchaseRequestRequestValidator_WithEmptyItems_ShouldFail()
    {
        CreatePurchaseRequestRequestValidator validator = new();
        CreatePurchaseRequestRequest model = new(1, "Title", "Justification", []);

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ApprovalDecisionRequestValidator_WithLongComment_ShouldFail()
    {
        ApprovalDecisionRequestValidator validator = new();
        ApprovalDecisionRequest model = new(2, new string('a', 501));

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void PurchaseRequestActionRequestValidator_WithInvalidUser_ShouldFail()
    {
        PurchaseRequestActionRequestValidator validator = new();
        PurchaseRequestActionRequest model = new(0, "comment");

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
    }
}
