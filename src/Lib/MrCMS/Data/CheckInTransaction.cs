using Microsoft.EntityFrameworkCore;

namespace MrCMS.Data
{
    public class CheckInTransaction : ICheckInTransaction
    {
        public bool IsInTransaction(DbContext context)
        {
            return context.Database.CurrentTransaction != null;
        }
    }
}