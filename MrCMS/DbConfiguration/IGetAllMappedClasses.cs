using System;
using System.Collections.Generic;

namespace MrCMS.DbConfiguration
{
    public interface IGetAllMappedClasses
    {
        List<Type> MappedClasses { get; }
    }

    public class AllMappedClasses : IGetAllMappedClasses
    {
        public AllMappedClasses(List<Type> mappedClasses)
        {
            MappedClasses = mappedClasses;
        }

        public List<Type> MappedClasses { get; private set; }
    }
}