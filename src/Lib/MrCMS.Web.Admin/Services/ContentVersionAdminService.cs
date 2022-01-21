using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models.Content;
using MrCMS.Web.Admin.Services.Content;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Web.Admin.Services;

public class ContentVersionAdminService : IContentVersionAdminService
{
    private readonly ISession _session;

    public ContentVersionAdminService(ISession session)
    {
        _session = session;
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
        return new ContentVersionModel
        {
            Id = version.Id,
            WebpageId = version.Webpage.Id,
            PreviewUrl = $"/{version.Webpage.UrlSegment}?version={version.Id}",
            Blocks = version.Blocks.Select(x =>
            {
                var blockModel = x.DeserializeData();
                var contentVersionBlockSummaryModel = new ContentVersionBlockSummaryModel
                {
                    Id = x.Id,
                    Type = ContentEditorTypeMappings.BlockTypes[x.Type],
                    TypeName = ContentEditorTypeMappings.BlockNames[x.Type],
                    Guid = x.Guid,
                    Items = blockModel.Items.Select(y => new ContentVersionBlockItemSummaryModel
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Type = y.GetType().FullName
                    }).ToList()
                };
                return contentVersionBlockSummaryModel;
            }).ToList()
        };
    }
}