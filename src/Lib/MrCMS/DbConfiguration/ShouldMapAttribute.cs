using System;

namespace MrCMS.DbConfiguration
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ShouldMapAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class ShouldMapEntityAttribute : Attribute
    {
    }
}