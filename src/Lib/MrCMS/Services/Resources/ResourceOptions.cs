using System.Globalization;

namespace MrCMS.Services.Resources;

public class ResourceOptions : PlainResourceOptions
{
    internal ResourceOptions()
    {
    }

    internal bool Editable { get; private set; }
    internal bool Html { get; private set; }

    internal ResourceOptions EnableInlineEditing()
    {
        Editable = true;
        return this;
    }

    public ResourceOptions DisableInlineEditing()
    {
        Editable = false;
        return this;
    }
    public ResourceOptions EnableHtmlEditing()
    {
        Html = true;
        return this;
    }

    public override ResourceOptions AddReplacement(string key, string value)
    {
        _replacements.Add(key, value);
        return this;
    }

    public override ResourceOptions SetDefaultValue(string defaultValue)
    {
        base.SetDefaultValue(defaultValue);
        return this;
    }

    public override ResourceOptions WithCulture(CultureInfo culture)
    {
        base.WithCulture(culture);
        return this;
    }
}