using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models.Content;
using NHibernate;

namespace MrCMS.Web.Admin.Services.Content;

public class ContentBlockAdminService : IContentBlockAdminService
{
    private readonly ISession _session;
    private readonly IServiceProvider _serviceProvider;


    public ContentBlockAdminService(ISession session, IServiceProvider serviceProvider)
    {
        _session = session;
        _serviceProvider = serviceProvider;
    }

    public Task<IReadOnlyList<ContentBlockOption>> GetContentRowOptions()
    {
        return Task.FromResult<IReadOnlyList<ContentBlockOption>>(ContentEditorTypeMappings.BlockOptions);
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
        if (!ContentEditorTypeMappings.BlockTypes.ContainsKey(model.BlockType))
            return null;

        var type = ContentEditorTypeMappings.BlockTypes[model.BlockType];

        var version = await _session.GetAsync<ContentVersion>(model.ContentVersionId);
        var instance = Activator.CreateInstance(type);

        var contentBlock = new ContentBlock
        {
            Order = version.Blocks.Any() ? version.Blocks.Max(x => x.Order) + 1 : 1,
            ContentVersion = version
        };
        contentBlock.SerializeData(instance);
        version.Blocks.Add(contentBlock);

        await _session.TransactAsync(session => session.SaveAsync(contentBlock));

        return contentBlock;
    }

    public async Task<IContentBlock> GetBlock(int id)
    {
        var contentBlock = await GetContentBlock(id);
        return contentBlock?.DeserializeData();
    }


    public async Task<object> GetUpdateModel(int id)
    {
        var contentBlock = await GetContentBlock(id);
        var configuration = GetAdminConfiguration(contentBlock);
        return configuration.GetEditModel(contentBlock.DeserializeData());
    }

    public async Task UpdateBlock(int id, object model)
    {
        var contentBlock = await GetContentBlock(id);
        var configuration = GetAdminConfiguration(contentBlock);
        if (model.GetType() != configuration.EditModelType)
            return;
        var block = contentBlock?.DeserializeData();

        configuration.UpdateBlock(block, model);
        contentBlock!.SerializeData(block);

        await _session.TransactAsync(session => session.UpdateAsync(contentBlock));
    }

    private async Task<ContentBlock> GetContentBlock(int id)
    {
        return await _session.GetAsync<ContentBlock>(id);
    }

    private async Task<IContentBlockAdminConfiguration> GetAdminConfiguration(int id)
    {
        var contentBlock = await _session.GetAsync<ContentBlock>(id);
        return GetAdminConfiguration(contentBlock);
    }

    private IContentBlockAdminConfiguration GetAdminConfiguration(ContentBlock contentBlock)
    {
        return _serviceProvider.GetService(ContentEditorTypeMappings.BlockConfigurations[contentBlock.Type]) as
            IContentBlockAdminConfiguration;
    }
}