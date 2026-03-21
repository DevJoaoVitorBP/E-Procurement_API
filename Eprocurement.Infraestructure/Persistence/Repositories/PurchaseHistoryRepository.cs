using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eprocurement.Infrastructure.Persistence.Repositories
{
    public class PurchaseHistoryRepository : IPurchaseHistoryRepository
    {
        private readonly ProcurementDbContext _context;

        public PurchaseHistoryRepository(ProcurementDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(PurchaseHistory history, CancellationToken cancellationToken = default)
            => _context.PurchaseHistories.AddAsync(history, cancellationToken).AsTask();

        public async Task<IReadOnlyCollection<PurchaseHistory>> GetByPurchaseRequestIdAsync(int purchaseRequestId, CancellationToken cancellationToken = default)
            => await _context.PurchaseHistories
                .AsNoTracking()
                .Where(x => x.PurchaseRequestId == purchaseRequestId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync(cancellationToken);
    }
}