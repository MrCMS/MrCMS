using System;
using MrCMS.Entities.People;

namespace MrCMS.TextSearch.EntityConverters
{
    public class UserTextSearchConverter : BaseTextSearchEntityConverter<User>
    {
        public override Type BaseType => typeof(User);

        protected override string LoadText(User entity)
        {
            return $"{entity.Email} {entity.FirstName} {entity.LastName}";
        }

        protected override string LoadDisplayName(User entity)
        {
            return entity.Name;
        }
    }
}