using System;
using System.Collections.Generic;

namespace MrCMS.DbConfiguration
{
    public interface IGetAllMappedClasses
    {
        List<Type> MappedClasses { get; }
    }
}