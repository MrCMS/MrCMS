using System;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Documents.Web;

[AttributeUsage(AttributeTargets.Class , AllowMultiple = false)]
public sealed class AllowedPageTypesAttribute : Attribute
{
    public AllowedPageTypesAttribute(params Type[] types)
    {
        Types = types;
    }
    
    public Type[] Types { get; private set; }
    
}