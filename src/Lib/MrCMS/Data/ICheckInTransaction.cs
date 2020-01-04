using Microsoft.EntityFrameworkCore;

namespace MrCMS.Data
{
    public interface ICheckInTransaction
    {
        bool IsInTransaction(DbContext context);
    }
}