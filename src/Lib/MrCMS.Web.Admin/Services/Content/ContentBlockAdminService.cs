using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models.Content;
using NHibernate;

namespace MrCMS.Web.Admin.Services.Content;

public class ContentBlockAdminService : IContentBlockAdminService
{
    private readonly ISession _session;
    private static readonly List<ContentBlockOption> RowOptions;
    private static readonly IReadOnlyDictionary<string, Type> RowTypes;

    static ContentBlockAdminService()
    {
        RowTypes = TypeHelper.GetAllConcreteTypesAssignableFrom<IContentBlock>().ToDictionary(x => x.FullName);
        var rowOptions = new List<ContentBlockOption>();
        foreach (var (fullName, rowType) in RowTypes)
        {
            var attribute = rowType.GetCustomAttribute<ContentBlockMetadataAttribute>();
            if (attribute == null)
                continue;
            rowOptions.Add(new ContentBlockOption
            {
                Name = attribute.DisplayName,
                TypeName = fullName,
                EditorType = attribute.EditorType
            });
        }

        RowOptions = rowOptions;
    }

    public ContentBlockAdminService(ISession session)
    {
        _session = session;
    }

    public Task<IReadOnlyList<ContentBlockOption>> GetContentRowOptions()
    {
        return Task.FromResult<IReadOnlyList<ContentBlockOption>>(RowOptions);
    }

    public async Task<AddContentBlockModel> GetAddModel(int id)
    {
        return new AddContentBlockModel
        {
            ContentVersionId = id,
            BlockType = string.Empty
        };
    }

    public async Task<ContentBlock> AddBlock(AddContentBlockModel model)
    {
        if (!RowTypes.ContainsKey(model.BlockType))
            return null;

        var type = RowTypes[model.BlockType];

        var version = await _session.GetAsync<ContentVersion>(model.ContentVersionId);
        var instance = Activator.CreateInstance(type);

        var contentBlock = new ContentBlock
        {
            Name = model.Name,
            Order = version.Blocks.Any() ? version.Blocks.Max(x => x.Order) + 1 : 1,
            ContentVersion = version
        };
        contentBlock.SerializeData(instance);
        version.Blocks.Add(contentBlock);

        await _session.TransactAsync(session => session.SaveAsync(contentBlock));

        return contentBlock;
    }
}