namespace MrCMS.Entities.Documents.Web;

public interface IContentBlockWithChildCollection : IContentBlock
{
    BlockItem AddChild();
    void Remove(BlockItem item);
}