using System;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Infrastructure.Services.Content;

public interface IContentBlockAdminConfiguration
{
    Type EditModelType { get; }
    object GetEditModel(IContentBlock contentBlock);
    void UpdateBlock(IContentBlock contentBlock, object editModel);
}