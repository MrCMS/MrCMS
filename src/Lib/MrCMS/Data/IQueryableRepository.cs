using System.Linq;

namespace MrCMS.Data
{
    public interface IQueryableRepository<T> where T : class
    {
        /// <summary>
        ///     IQueryable that returns data tracked (for data manipulation)
        /// </summary>
        /// <returns></returns>
        IQueryable<T> Query();

        /// <summary>
        ///     IQueryable that returns data tracked (for data manipulation)
        /// </summary>
        /// <returns></returns>
        IQueryable<TSubtype> Query<TSubtype>() where TSubtype : class, T;

        /// <summary>
        ///     IQueryable that returns data untracked (for display)
        /// </summary>
        /// <returns></returns>
        IQueryable<T> Readonly();

        /// <summary>
        ///     IQueryable that returns data untracked (for display)
        /// </summary>
        /// <returns></returns>
        IQueryable<TSubtype> Readonly<TSubtype>() where TSubtype : class, T;
    }
}