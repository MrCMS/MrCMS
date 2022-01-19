using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models.Content;
using NHibernate.Mapping;

namespace MrCMS.Web.Admin.Services.Content;

public class ContentRowAdminService : IContentRowAdminService
{
    private static readonly List<ContentRowOption> RowOptions;

    static ContentRowAdminService()
    {
        var rowTypes = TypeHelper.GetAllConcreteTypesAssignableFrom<IContentRow>();
        var rowOptions = new List<ContentRowOption>();
        foreach (var rowType in rowTypes)
        {
            var attribute = rowType.GetCustomAttribute<ContentRowMetadataAttribute>();
            if (attribute == null)
                continue;
            rowOptions.Add(new ContentRowOption
            {
                Name = attribute.DisplayName,
                EditorType = attribute.EditorType
            });
        }

        RowOptions = rowOptions;
    }

    public Task<IReadOnlyList<ContentRowOption>> GetContentRowOptions()
    {
        return Task.FromResult<IReadOnlyList<ContentRowOption>>(RowOptions);
    }
}