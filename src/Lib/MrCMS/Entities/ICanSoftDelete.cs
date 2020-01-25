namespace MrCMS.Entities
{
    public interface ICanSoftDelete
    {
        bool IsDeleted { get; set; }
    }
}