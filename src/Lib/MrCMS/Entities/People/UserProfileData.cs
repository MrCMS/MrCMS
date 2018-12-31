using System;
using System.Collections.Generic;
using MrCMS.Helpers;

namespace MrCMS.Entities.People
{
    public abstract class UserProfileData : SystemEntity
    {
        public virtual User User { get; set; }

        public static IEnumerable<Type> Types
        {
            get { return TypeHelper.GetAllConcreteMappedClassesAssignableFrom<UserProfileData>(); }
        }
    }
}