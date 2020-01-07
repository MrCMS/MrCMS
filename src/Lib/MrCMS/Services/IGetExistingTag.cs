using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    public interface IGetExistingTag
    {
        Tag GetTag(string name);
    }
}