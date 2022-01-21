using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models.Content;

namespace MrCMS.Web.Admin.Services.Content;

public static class ContentEditorTypeMappings
{
    public static readonly List<ContentBlockOption> BlockOptions;
    public static readonly IReadOnlyDictionary<string, Type> BlockTypes;
    public static readonly IReadOnlyDictionary<string, string> BlockNames;
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
        var missingBlockTypeConfigs = BlockTypes.Keys.Where(x => !BlockConfigurations.ContainsKey(x)).ToList();
        if (missingBlockTypeConfigs.Any())
        {
            throw new InvalidOperationException(
                $"Cannot start without configs for: {string.Join(", ", missingBlockTypeConfigs)}");
        }

        var blockOptions = new List<ContentBlockOption>();
        var blockNames = new Dictionary<string, string>();
        foreach (var (fullName, rowType) in BlockTypes)
        {
            var attribute = rowType.GetCustomAttribute<DisplayAttribute>();
            var name = attribute?.Name ?? rowType.Name.BreakUpString();
            blockNames[fullName] = name;
            blockOptions.Add(new ContentBlockOption
            {
                Name = name,
                TypeName = fullName,
            });
        }

        BlockOptions = blockOptions;
        BlockNames = blockNames;

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