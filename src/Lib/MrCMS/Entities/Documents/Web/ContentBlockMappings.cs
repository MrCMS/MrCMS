using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using MrCMS.Helpers;

namespace MrCMS.Entities.Documents.Web;

public static class ContentBlockMappings
{
    public static readonly IReadOnlyDictionary<string, ContentBlockMetadata> BlockMetadata;

    static ContentBlockMappings()
    {
        var blockTypes = TypeHelper.GetAllConcreteTypesAssignableFrom<IContentBlock>();

        var blockMetadata = new Dictionary<string, ContentBlockMetadata>();
        foreach (var type in blockTypes)
        {
            if (type.FullName == null)
                continue;

            var attribute = type.GetCustomAttribute<DisplayAttribute>();

            var name = attribute?.Name ?? type.Name.BreakUpString();
            var canAddChildren = type.IsAssignableTo(typeof(IContentBlockWithChildCollection));
                blockMetadata.Add(type.FullName!, new ContentBlockMetadata
                {
                    Name = name,
                    Type = type,
                    CanAddChildren = canAddChildren
                });
        }

        BlockMetadata = blockMetadata;
    }

    public class ContentBlockMetadata
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public bool CanAddChildren { get; set; }
    }
}