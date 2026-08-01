namespace Bussines_Library.Domain.Common
{
    public abstract class AuditableEntity
    {
        public DateTime CreatedAtUtc { get; set; }
        public string? CreatedBy { get; set; } = null;
        public DateTime? UpdateAtUtc { get; set; }
        public string? UpdatedBy { get; set; } = null;

        public void MarkCreated(DateTime createdAtUtc, string? createdBy)
        {
            CreatedAtUtc = createdAtUtc;
            CreatedBy = createdBy;
            UpdatedBy = createdBy;

        }

        public void MarkUpdated(DateTime? updateAtUtc, string? updatedBy)
        {
            UpdateAtUtc = updateAtUtc;
            UpdatedBy = updatedBy;
        }

        private static DateTime EnsureUtc(DateTime date) => date.Kind == DateTimeKind.Utc ? date : DateTime.SpecifyKind(date, DateTimeKind.Utc);

    }
}
