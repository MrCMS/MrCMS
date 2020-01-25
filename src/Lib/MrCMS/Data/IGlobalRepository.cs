using MrCMS.Entities;

namespace MrCMS.Data
{
    public interface IGlobalRepository<T> : IRepositoryBase<T, IGlobalRepository<T>> where T : class
    {
    }
}