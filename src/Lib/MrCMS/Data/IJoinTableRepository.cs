namespace MrCMS.Data
{
    public interface IJoinTableRepository<T> : IRepositoryBase<T, IJoinTableRepository<T>> where T : class, IJoinTable
    {
    }
}