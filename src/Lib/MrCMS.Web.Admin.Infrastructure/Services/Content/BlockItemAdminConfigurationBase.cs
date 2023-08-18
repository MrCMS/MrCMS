using System;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Infrastructure.Services.Content;

public abstract class BlockItemAdminConfigurationBase<TBlockItem, TEditModel> : IBlockItemAdminConfiguration
    where TBlockItem : BlockItem, new()
{
    public abstract TEditModel GetEditModel(TBlockItem block);

    public Type EditModelType => typeof(TEditModel);

    object IBlockItemAdminConfiguration.GetEditModel(BlockItem blockItem)
    {
        return GetEditModel(blockItem is TBlockItem block ? block : default);
    }

    public abstract void UpdateBlockItem(TBlockItem block, TEditModel editModel);

    void IBlockItemAdminConfiguration.UpdateBlockItem(BlockItem blockItem, object editModel)
    {
        UpdateBlockItem(blockItem is TBlockItem block ? block : default,
            editModel is TEditModel model ? model : default);
    }
}