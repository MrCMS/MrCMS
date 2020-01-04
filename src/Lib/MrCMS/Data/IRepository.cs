using MrCMS.Entities;

namespace MrCMS.Data
{
    public interface IRepository<T> : IRepositoryBase<T, IRepository<T>> where T : class, /*IHaveId,*/ IHaveSite
    {
    }
}