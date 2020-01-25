using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MrCMS.Data
{
    public class ChangeInfo : EntityData
    {
        public List<PropertyData> PropertiesUpdated
        {
            get
            {
                var properties = new List<PropertyData>();
                var originalValues = OriginalValues;
                var currentValues = Properties;

                foreach (var property in currentValues.Keys)
                {
                    var originalValue = originalValues[property];
                    var currentValue = currentValues[property];

                    if (!Equals(originalValue, currentValue))
                    {
                        properties.Add(new PropertyData
                        {
                            Name = property,
                            OriginalValue = originalValue,
                            CurrentValue = currentValue
                        });
                    }
                }

                return properties;
            }
        }
        public IImmutableDictionary<string, object> OriginalValues { get; set; }
    }
}