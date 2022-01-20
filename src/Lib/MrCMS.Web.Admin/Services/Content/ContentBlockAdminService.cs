using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public static readonly IReadOnlyDictionary<string, Type> RowTypes;
    public static readonly IReadOnlyDictionary<string, Type> BlockTypes;

    static ContentBlockAdminService()
    {
        RowTypes = TypeHelper.GetAllConcreteTypesAssignableFrom<IContentBlock>().ToDictionary(x => x.FullName);
        BlockTypes = TypeHelper.GetAllConcreteTypesAssignableFrom<BlockItem>().ToDictionary(x => x.FullName);

        var rowOptions = new List<ContentBlockOption>();
        foreach (var (fullName, rowType) in RowTypes)
        {
            var attribute = rowType.GetCustomAttribute<DisplayAttribute>();
            rowOptions.Add(new ContentBlockOption
            {
                Name = attribute?.Name ?? rowType.Name.BreakUpString(),
                TypeName = fullName,
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

    public async Task<string> GetName(int id)
    {
        var contentBlock = await _session.GetAsync<ContentBlock>(id);
        return contentBlock?.Name;
    }

    public async Task<IContentBlock> GetBlock(int id)
    {
        var contentBlock = await _session.GetAsync<ContentBlock>(id);
        return contentBlock?.DeserializeData();
    }
}