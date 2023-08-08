using System;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;

namespace MrCMS.DbConfiguration.Configuration
{
    public class SaveWebpageVersions : IOnUpdated<Webpage>
    {
        private async Task<User> GetUser(ISession session)
        {
            return await session.GetService<IGetCurrentUser>().Get();
        }

        public async Task Execute(OnUpdatedArgs<Webpage> args)
        {
            var webpage = args.Item;
            if (webpage is { IsDeleted: false } && args.Original != null)
            {
                var propertyInfos = webpage.GetType().GetVersionProperties();

                var jObject = new JObject();

                var anyChanges = false;
                foreach (var propertyInfo in propertyInfos)
                {
                    if (propertyInfo == null)
                        continue;

                    var oldValue = propertyInfo.GetValue(args.Original) ??
                                   (propertyInfo.PropertyType.IsValueType
                                        ? Activator.CreateInstance(propertyInfo.PropertyType)
                                        : null);
                    var newValue = propertyInfo.GetValue(args.Item);

                    if (oldValue != null)
                        if (!oldValue.Equals(newValue))
                            anyChanges = true;

                    if (oldValue == null && newValue != null)
                        anyChanges = true;

                    jObject.Add(propertyInfo.Name, new JRaw(JsonConvert.SerializeObject(oldValue)));

                }
                if (anyChanges)
                {
                    var s = args.Session;
                    var documentVersion = new WebpageVersion
                    {
                        Webpage = webpage,
                        Data = JsonConvert.SerializeObject(jObject),
                        User = await GetUser(s),
                        Site = webpage.Site
                    };
                    webpage.Versions.Add(documentVersion);
                    await s.TransactAsync((session,token) => session.SaveAsync(documentVersion, token));
                }
            }
        }
    }
}
