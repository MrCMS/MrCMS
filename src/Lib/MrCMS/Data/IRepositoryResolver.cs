using MrCMS.Entities;

namespace MrCMS.Data
{
    public interface IRepositoryResolver
    {
        IRepository<T> GetRepository<T>() where T : class, IHaveId, IHaveSite;
        IGlobalRepository<T> GetGlobalRepository<T>() where T : class, IHaveId;
    }
}