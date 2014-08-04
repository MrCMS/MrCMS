using MrCMS.Entities;

namespace MrCMS.Events
{
    public interface IOnUpdating<T>: IEvent<OnUpdatingArgs<T>> where T : SystemEntity
    {
    }

    public interface IOnUpdated<T>: IEvent<OnUpdatedArgs<T>> where T : SystemEntity
    {
    }
}