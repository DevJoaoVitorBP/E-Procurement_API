namespace Eprocurement.Application.Contracts
{
    public record CreateSupplierRequest(string CorporateName, string DocumentNumber, string Email, string Phone);

    public record SupplierResponse(int Id, string CorporateName, string DocumentNumber, string Email, string Phone, bool IsActive);
}