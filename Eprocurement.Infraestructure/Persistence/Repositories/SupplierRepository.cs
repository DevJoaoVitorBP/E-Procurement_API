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

        public async Task<IReadOnlyCollection<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.Suppliers.AsNoTracking().OrderBy(x => x.CorporateName).ToListAsync(cancellationToken);

        public async Task<(IReadOnlyCollection<Supplier> Items, int TotalCount)> GetPagedAsync(
            string? searchTerm,
            string? documentNumber,
            bool? isActive,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Supplier> query = _context.Suppliers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim();
                query = query.Where(x =>
                    EF.Functions.Like(x.CorporateName, $"%{term}%") ||
                    EF.Functions.Like(x.Email, $"%{term}%"));
            }

            if (!string.IsNullOrWhiteSpace(documentNumber))
            {
                string doc = documentNumber.Trim();
                query = query.Where(x => x.DocumentNumber == doc);
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            int totalCount = await query.CountAsync(cancellationToken);

            List<Supplier> items = await query
                .OrderBy(x => x.CorporateName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _context.Suppliers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public void Update(Supplier supplier) => _context.Suppliers.Update(supplier);

        public void Delete(Supplier supplier) => _context.Suppliers.Remove(supplier);

        public Task DeleteAsync(Supplier supplier, CancellationToken cancellationToken = default)
        {
            _context.Suppliers.Remove(supplier);
            return Task.CompletedTask;
        }
    }
}