using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eprocurement.Infrastructure.Persistence.Repositories
{
    public class PurchaseRequestRepository : IPurchaseRequestRepository
    {
        private readonly ProcurementDbContext _context;

        public PurchaseRequestRepository(ProcurementDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(PurchaseRequest request, CancellationToken cancellationToken = default)
            => _context.PurchaseRequests.AddAsync(request, cancellationToken).AsTask();

        public Task<PurchaseRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => _context.PurchaseRequests
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IReadOnlyCollection<PurchaseRequest>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.PurchaseRequests
                .AsNoTracking()
                .Include(x => x.Items)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync(cancellationToken);

        public void Update(PurchaseRequest request) => _context.PurchaseRequests.Update(request);
    }
}