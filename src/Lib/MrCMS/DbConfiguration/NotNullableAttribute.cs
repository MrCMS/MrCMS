using System;

namespace MrCMS.DbConfiguration
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NotNullableAttribute : Attribute
    {
    }
}