using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eprocurement.Infrastructure.Persistence.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ProcurementDbContext _context;

        public SupplierRepository(ProcurementDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
            => _context.Suppliers.AddAsync(supplier, cancellationToken).AsTask();

        public Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => _context.Suppliers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IReadOnlyCollection<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.Suppliers.AsNoTracking().OrderBy(x => x.CorporateName).ToListAsync(cancellationToken);

        public void Update(Supplier supplier) => _context.Suppliers.Update(supplier);

        public void Remove(Supplier supplier) => _context.Suppliers.Remove(supplier);
    }
}