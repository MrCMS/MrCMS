using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Infrastructure.Services.Content;
using MrCMS.Web.Admin.Models.Content;
using NHibernate;

namespace MrCMS.Web.Admin.Services.Content;

public class ContentBlockItemAdminService : IContentBlockItemAdminService
{
    private readonly ISession _session;
    private readonly IServiceProvider _serviceProvider;

    public ContentBlockItemAdminService(ISession session, IServiceProvider serviceProvider)
    {
        _session = session;
        _serviceProvider = serviceProvider;
    }

    public async Task<BlockItem> GetBlockItem(int blockId, Guid itemId)
    {
        var contentBlock = await GetContentBlock(blockId);
        if (contentBlock == null)
            return null;

        var block = contentBlock.DeserializeData();
        return block.Items.FirstOrDefault(x => x.Id == itemId);
    }

    public async Task<object> GetUpdateModel(int blockId, Guid itemId)
    {
        var blockItem = await GetBlockItem(blockId, itemId);
        var configuration = GetAdminConfiguration(blockItem);
        return configuration.GetEditModel(blockItem);
    }

    public async Task UpdateBlockItem(int blockId, Guid itemId, object model)
    {
        var contentBlock = await GetContentBlock(blockId);
        if (contentBlock == null)
            return;

        var block = contentBlock.DeserializeData();
        var blockItem = block.Items.FirstOrDefault(x => x.Id == itemId);
        var configuration = GetAdminConfiguration(blockItem);
        configuration.UpdateBlockItem(blockItem, model);

        contentBlock.SerializeData(block);
        await _session.TransactAsync(session => session.UpdateAsync(contentBlock));
    }

    public async Task RemoveBlockItem(int blockId, Guid itemId)
    {
        var contentBlock = await GetContentBlock(blockId);
        if (contentBlock == null)
            return;

        var block = contentBlock.DeserializeData();
        if (block is not IContentBlockWithChildCollection withChildCollection)
            return;
        var blockItem = withChildCollection.Items.FirstOrDefault(x => x.Id == itemId);
        if (blockItem == null)
            return;
        withChildCollection.Remove(blockItem);

        contentBlock.SerializeData(withChildCollection);
        await _session.TransactAsync(session => session.UpdateAsync(contentBlock));
    }

    public async Task SetBlockItemOrders(int blockId, List<BlockItemSortModel> blockItemSortModel)
    {
        var contentBlock = await GetContentBlock(blockId);
        if (contentBlock == null)
            return;

        var block = contentBlock.DeserializeData();
        if (block is not IContentBlockWithSortableChildCollection withSortableChildCollection)
            return;

        withSortableChildCollection.Sort(blockItemSortModel.Select(f => new KeyValuePair<Guid, int>(f.Id, f.Order)).ToList());

        contentBlock.SerializeData(withSortableChildCollection);
        await _session.TransactAsync(session => session.UpdateAsync(contentBlock));
    }

    private async Task<ContentBlock> GetContentBlock(int id)
    {
        return await _session.GetAsync<ContentBlock>(id);
    }


    private IBlockItemAdminConfiguration GetAdminConfiguration(BlockItem blockItem)
    {
        return _serviceProvider.GetService(
                ContentEditorTypeMappings.BlockItemConfigurations[blockItem.GetType().FullName]) as
            IBlockItemAdminConfiguration;
    }
}