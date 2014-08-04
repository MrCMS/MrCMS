using MrCMS.Entities;

namespace MrCMS.Events
{
    public interface IOnAdding<T>: IEvent<OnAddingArgs<T>> where T : SystemEntity
    {
    }
    public interface IOnAdded<T>: IEvent<OnAddedArgs<T>> where T : SystemEntity
    {
    }
}