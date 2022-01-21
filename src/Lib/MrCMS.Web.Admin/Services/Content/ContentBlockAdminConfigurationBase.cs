using System;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Services.Content;

public abstract class ContentBlockAdminConfigurationBase<TBlock, TEditModel> : IContentBlockAdminConfiguration
    where TBlock : IContentBlock
{
    public abstract TEditModel GetEditModel(TBlock block);

    public Type EditModelType => typeof(TEditModel);

    object IContentBlockAdminConfiguration.GetEditModel(IContentBlock contentBlock)
    {
        return GetEditModel(contentBlock is TBlock block ? block : default);
    }

    public abstract void UpdateBlock(TBlock block, TEditModel editModel);

    void IContentBlockAdminConfiguration.UpdateBlock(IContentBlock contentBlock, object editModel)
    {
        UpdateBlock(contentBlock is TBlock block ? block : default, editModel is TEditModel model ? model : default);
    }
}