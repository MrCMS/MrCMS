using System;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Infrastructure.Services.Content;

public interface IBlockItemAdminConfiguration
{
    Type EditModelType { get; }
    object GetEditModel(BlockItem blockItem);
    void UpdateBlockItem(BlockItem blockItem, object editModel);
}