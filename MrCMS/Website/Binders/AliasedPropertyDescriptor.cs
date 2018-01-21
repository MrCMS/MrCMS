using System;
using System.ComponentModel;

namespace MrCMS.Website.Binders
{
    internal sealed class AliasedPropertyDescriptor : PropertyDescriptor
    {
        public PropertyDescriptor Inner { get; }

        public AliasedPropertyDescriptor(string alias, PropertyDescriptor inner)
            : base(alias, null)
        {
            Inner = inner;
        }

        public override bool CanResetValue(object component)
        {
            return Inner.CanResetValue(component);
        }

        public override Type ComponentType => Inner.ComponentType;

        public override object GetValue(object component)
        {
            return Inner.GetValue(component);
        }

        public override bool IsReadOnly => Inner.IsReadOnly;

        public override Type PropertyType => Inner.PropertyType;

        public override void ResetValue(object component)
        {
            Inner.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            Inner.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return Inner.ShouldSerializeValue(component);
        }
    }
}