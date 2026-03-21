using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eprocurement.Infrastructure.Persistence.Repositories
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly ProcurementDbContext _context;

        public PurchaseOrderRepository(ProcurementDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(PurchaseOrder order, CancellationToken cancellationToken = default)
            => _context.PurchaseOrders.AddAsync(order, cancellationToken).AsTask();

        public Task<PurchaseOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => _context.PurchaseOrders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IReadOnlyCollection<PurchaseOrder>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.PurchaseOrders
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync(cancellationToken);
    }
}