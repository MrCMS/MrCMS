using System;

namespace MrCMS.Website.Binders
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class BindAliasAttribute : Attribute
    {
        public BindAliasAttribute(string alias)
        {
            //ommitted: parameter checking
            Alias = alias;
        }

        public string Alias { get; }
    }
}