namespace MrCMS.Models
{
    public interface IAdminMenuItem 
    {
        string Text { get; }
        string Url { get; }
        bool CanShow { get; }

        SubMenu Children { get; }
        int DisplayOrder { get; }
    }
}