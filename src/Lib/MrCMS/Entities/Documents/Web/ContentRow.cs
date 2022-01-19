using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using Newtonsoft.Json;

namespace MrCMS.Entities.Documents.Web;

public interface IContentRow
{
    IReadOnlyList<ContentArea> Areas { get; }
}

public class ContentRow : SiteEntity
{
    private static readonly JsonSerializerSettings ContentRowSerializerSettings = new();
    
    public virtual string Name { get; set; }
    public virtual int Order { get; set; }

    public virtual string Type { get; set; }
    public virtual string Data { get; set; }

    public virtual void SerializeData(object model)
    {
        var type = TypeHelper.GetTypeByName(Type);
        if (type == null)
            return;
        Data = JsonConvert.SerializeObject(model, type, Formatting.None, ContentRowSerializerSettings);
    }

    public virtual object DeserializeData()
    {
        var type = TypeHelper.GetTypeByName(Type);
        if (type == null)
            return null;
        return JsonConvert.DeserializeObject(Data, type, ContentRowSerializerSettings);
    }
}

public class ContentRowMetadataAttribute : Attribute
{
    public ContentRowMetadataAttribute(string displayName, string editorType)
    {
        DisplayName = displayName;
        EditorType = editorType;
    }

    public string DisplayName { get; }
    public string EditorType { get; }
}