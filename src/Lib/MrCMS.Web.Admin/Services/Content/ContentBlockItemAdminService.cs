using System;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using NHibernate;

namespace MrCMS.Web.Admin.Services.Content;

public class ContentBlockItemAdminService : IContentBlockItemAdminService
{
    private readonly ISession _session;

    public ContentBlockItemAdminService(ISession session)
    {
        _session = session;
    }
    
    public async Task<BlockItem> GetBlockItem(int blockId, Guid itemId)
    {
        var contentBlock = await _session.GetAsync<ContentBlock>(blockId);
        if (contentBlock == null)
            return null;

        var block = contentBlock.DeserializeData();
        return block.Items.FirstOrDefault(x => x.Id == itemId);
    }
}