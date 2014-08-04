using MrCMS.Entities;

namespace MrCMS.Events
{
    public interface IOnDeleting<T>: IEvent<OnDeletingArgs<T>> where T : SystemEntity
    {
    }
    public interface IOnDeleted<T>: IEvent<OnDeletedArgs<T>> where T : SystemEntity
    {
    }
}