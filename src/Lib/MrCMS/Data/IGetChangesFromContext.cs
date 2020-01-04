using Microsoft.EntityFrameworkCore;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public interface IGetChangesFromContext
    {
        ContextChangeData GetChanges<T>(DbContext context) where T : class;
    }
}