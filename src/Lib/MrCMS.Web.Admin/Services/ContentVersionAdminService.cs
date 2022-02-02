using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Admin.Models.Content;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MrCMS.Web.Admin.Services;

public class ContentVersionAdminService : IContentVersionAdminService
{
    private readonly ISession _session;
    private readonly IGetHomePage _getHomePage;

    public ContentVersionAdminService(ISession session, IGetHomePage getHomePage)
    {
        _session = session;
        _getHomePage = getHomePage;
    }

    public async Task<IReadOnlyList<ContentVersion>> GetVersions(int webpageId)
    {
        return await _session.Query<ContentVersion>()
            .Where(x => x.Webpage.Id == webpageId)
            .OrderByDescending(x => x.CreatedOn)
            .ToListAsync();
    }

    public async Task<ContentVersion> AddInitialContentVersion(AddInitialContentVersionModel model)
    {
        var webpage = _session.Get<Webpage>(model.WebpageId);
        if (webpage == null)
            return null;

        var contentVersion = new ContentVersion { Webpage = webpage, Status = ContentVersionStatus.Draft };
        webpage.ContentVersions.Add(contentVersion);
        await _session.TransactAsync(session => session.SaveAsync(contentVersion));

        return contentVersion;
    }

    public async Task<ContentVersionModel> GetEditModel(int id)
    {
        var version = await _session.GetAsync<ContentVersion>(id);

        var previewUrl = $"/{version.Webpage.UrlSegment}?version={version.Id}";
        if ((await _getHomePage.Get()).Id == version.Webpage.Id)
        {
            previewUrl = $"/?version={version.Id}";
        }

        return new ContentVersionModel
        {
            Id = version.Id,
            WebpageId = version.Webpage.Id,
            PreviewUrl = previewUrl,
            Blocks = version.Blocks.OrderBy(f => f.Order).Select(x =>
            {
                var blockModel = x.DeserializeData();
                var metadata = ContentBlockMappings.BlockMetadata[x.Type];
                var contentVersionBlockSummaryModel = new ContentVersionBlockSummaryModel
                {
                    Id = x.Id,
                    Order = x.Order,
                    IsHidden = x.IsHidden,
                    Type = metadata.Type,
                    TypeName = metadata.Name,
                    CanAddChildren = metadata.CanAddChildren,
                    Guid = x.Guid,
                    Items = blockModel.Items.Select(y => new ContentVersionBlockItemSummaryModel
                    {
                        Id = y.Id,
                        Name = y.GetDisplayName(blockModel),
                        Type = y.GetType().FullName
                    }).ToList()
                };
                return contentVersionBlockSummaryModel;
            }).ToList()
        };
    }
}