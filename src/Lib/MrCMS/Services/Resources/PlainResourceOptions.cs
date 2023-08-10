using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;

namespace MrCMS.Services.Resources;

public class PlainResourceOptions
{
    internal PlainResourceOptions()
    {
    }

    public virtual PlainResourceOptions AddReplacement(string key, string value)
    {
        _replacements.Add(key, value);
        return this;
    }

    public virtual PlainResourceOptions SetDefaultValue(string defaultValue)
    {
        DefaultValue = defaultValue;
        return this;
    }

    public virtual PlainResourceOptions WithCulture(CultureInfo culture)
    {
        Culture = culture;
        return this;
    }

    internal string DefaultValue { get;  private set; }
    internal IReadOnlyDictionary<string, string> Replacements => _replacements;
    protected Dictionary<string, string> _replacements = new();

    [CanBeNull] internal CultureInfo Culture { get; private set; }
}