namespace MrCMS.Events
{
    public interface IOnDeleting: IEvent<OnDeletingArgs>
    {
    }
    public interface IOnDeleted: IEvent<OnDeletedArgs>
    {
    }
}