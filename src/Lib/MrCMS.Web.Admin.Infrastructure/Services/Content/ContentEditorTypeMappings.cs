using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Web.Admin.Infrastructure.Services.Content;

public static class ContentEditorTypeMappings
{
    public static readonly IReadOnlyDictionary<string, Type> BlockTypes;
    public static readonly IReadOnlyDictionary<string, Type> BlockConfigurations;

    public static readonly IReadOnlyDictionary<string, Type> BlockItemTypes;
    public static readonly IReadOnlyDictionary<string, Type> BlockItemConfigurations;

    static ContentEditorTypeMappings()
    {
        var blockTypes = TypeHelper.GetAllConcreteTypesAssignableFrom<IContentBlock>();
        var blockConfigTypes =
            TypeHelper.GetAllConcreteTypesAssignableFromGeneric(typeof(ContentBlockAdminConfigurationBase<,>));
        
        BlockTypes = blockTypes.ToDictionary(x => x.FullName);

        BlockConfigurations = blockConfigTypes.Select(x =>
            new
            {
                baseType = x.GetBaseTypes().First(y =>
                    y.IsGenericType && y.GetGenericTypeDefinition() == typeof(ContentBlockAdminConfigurationBase<,>)),
                type = x
            }).ToDictionary(x => x.baseType.GetGenericArguments()[0].FullName, x => x.type);
        var missingBlockTypeConfigs = blockTypes.Where(x => !BlockConfigurations.ContainsKey(x.FullName)).ToList();
        if (missingBlockTypeConfigs.Any())
        {
            throw new InvalidOperationException(
                $"Cannot start without configs for: {string.Join(", ", missingBlockTypeConfigs)}");
        }

        var itemTypes = TypeHelper.GetAllConcreteTypesAssignableFrom<BlockItem>();
        var blockItemConfigTypes =
            TypeHelper.GetAllConcreteTypesAssignableFromGeneric(typeof(BlockItemAdminConfigurationBase<,>));
        BlockItemTypes = itemTypes.ToDictionary(x => x.FullName);
        BlockItemConfigurations = blockItemConfigTypes.Select(x =>
            new
            {
                baseType = x.GetBaseTypes().First(y =>
                    y.IsGenericType && y.GetGenericTypeDefinition() == typeof(BlockItemAdminConfigurationBase<,>)),
                type = x
            }).ToDictionary(x => x.baseType.GetGenericArguments()[0].FullName, x => x.type);
        var missingBlockItemTypeConfigs =
            BlockItemTypes.Keys.Where(x => !BlockItemConfigurations.ContainsKey(x)).ToList();
        if (missingBlockItemTypeConfigs.Any())
        {
            throw new InvalidOperationException(
                $"Cannot start without configs for: {string.Join(", ", missingBlockItemTypeConfigs)}");
        }
    }
}