using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Domain.Common
{
    public class BaseAuditableEntity: BaseEntity
    {
        public DateTime CreatedOnUtc { get; protected set; }
        public string? CreatedBy { get; protected set; }

        public DateTime? ModifiedOnUtc { get; protected set; }
        public string? ModifiedBy { get; protected set; }

        public bool IsDeleted { get; protected set; }

        public void MarkAsModified(string? userId)
        {
            ModifiedOnUtc = DateTime.UtcNow;
            ModifiedBy = userId;
        }

        public void MarkAsCreated(string? userId)
        {
            CreatedOnUtc = DateTime.UtcNow;
            CreatedBy = userId;
        }

        public void SoftDelete(string? userId)
        {
            IsDeleted = true;
            ModifiedOnUtc = DateTime.UtcNow;
            ModifiedBy = userId;
        }
    }
}
