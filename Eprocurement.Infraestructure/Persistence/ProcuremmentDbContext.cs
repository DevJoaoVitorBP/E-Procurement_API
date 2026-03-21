using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eprocurement.Infrastructure.Persistence
{
    public class ProcurementDbContext : DbContext, IUnitOfWork
    {
        public ProcurementDbContext(DbContextOptions<ProcurementDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();
        public DbSet<PurchaseRequestItem> PurchaseRequestItems => Set<PurchaseRequestItem>();
        public DbSet<ApprovalStep> ApprovalSteps => Set<ApprovalStep>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<PurchaseHistory> PurchaseHistories => Set<PurchaseHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
                entity.Property(x => x.Email).HasMaxLength(150).IsRequired();
                entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
                entity.Property(x => x.Role).HasConversion<int>().IsRequired();
                entity.Property(x => x.IsActive).IsRequired();
                entity.HasIndex(x => x.Email).IsUnique();
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Suppliers");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.CorporateName).HasMaxLength(200).IsRequired();
                entity.Property(x => x.DocumentNumber).HasMaxLength(30).IsRequired();
                entity.Property(x => x.Email).HasMaxLength(150).IsRequired();
                entity.Property(x => x.Phone).HasMaxLength(30).IsRequired();
                entity.Property(x => x.IsActive).IsRequired();
                entity.HasIndex(x => x.DocumentNumber).IsUnique();
            });

            modelBuilder.Entity<PurchaseRequest>(entity =>
            {
                entity.ToTable("PurchaseRequests");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
                entity.Property(x => x.Justification).HasMaxLength(1000).IsRequired();
                entity.Property(x => x.Status).HasConversion<int>().IsRequired();

                entity.HasMany(x => x.Items)
                    .WithOne()
                    .HasForeignKey(x => x.PurchaseRequestId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PurchaseRequestItem>(entity =>
            {
                entity.ToTable("PurchaseRequestItems");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Description).HasMaxLength(300).IsRequired();
                entity.Property(x => x.Quantity).IsRequired();
                entity.Property(x => x.UnitPrice).HasPrecision(18, 2).IsRequired();
            });

            modelBuilder.Entity<ApprovalStep>(entity =>
            {
                entity.ToTable("ApprovalSteps");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Decision).HasConversion<int>().IsRequired();
                entity.Property(x => x.Comment).HasMaxLength(500);

                entity.HasOne<PurchaseRequest>()
                    .WithMany()
                    .HasForeignKey(x => x.PurchaseRequestId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.ToTable("PurchaseOrders");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.TotalAmount).HasPrecision(18, 2).IsRequired();
                entity.Property(x => x.Status).HasConversion<int>().IsRequired();
            });

            modelBuilder.Entity<PurchaseHistory>(entity =>
            {
                entity.ToTable("PurchaseHistories");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Action).HasMaxLength(200).IsRequired();
                entity.Property(x => x.PerformedBy).HasMaxLength(150).IsRequired();
                entity.Property(x => x.Notes).HasMaxLength(1000);

                entity.HasOne<PurchaseRequest>()
                    .WithMany()
                    .HasForeignKey(x => x.PurchaseRequestId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}