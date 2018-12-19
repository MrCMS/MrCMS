namespace MrCMS.Entities
{
    public interface ISortable
    {
        int DisplayOrder { get; set; }
        string DisplayName { get; }
    }
}