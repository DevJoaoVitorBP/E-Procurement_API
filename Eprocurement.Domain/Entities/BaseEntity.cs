namespace Eprocurement.Domain.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; protected set; }
        public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; protected set; }

        protected void Touch() => UpdatedAtUtc = DateTime.UtcNow;
    }
}