using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Resources;
using MrCMS.Services.Resources;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class StringResourceUpdateService : IStringResourceUpdateService
    {
        public const string Quote = "\"";
        private readonly IStringResourceProvider _provider;
        private readonly ISession _session;

        public StringResourceUpdateService(IStringResourceProvider provider, ISession session)
        {
            _provider = provider;
            _session = session;
        }

        public FileResult Export(StringResourceSearchQuery searchQuery)
        {
            var resources = _provider.AllResources.GetResourcesByKeyAndValue(searchQuery);
            var data = new List<List<string>>
                           {
                               new List<string>
                                   {
                                       "Key",
                                       "Value",
                                       "Culture",
                                       "SiteId"
                                   }
                           };
            foreach (var source in resources.OrderBy(resource => resource.Key))
            {
                var row = new List<string>();
                AddToRow(row, source.Key);
                AddToRow(row, source.Value);
                AddToRow(row, source.UICulture);
                AddToRow(row, source.Site.Id.ToString());
                data.Add(row);
            }
            string result = string.Join(Environment.NewLine, data.Select(list => string.Join(",", list)));

            return new FileContentResult(Encoding.Default.GetBytes(result), "text/csv")
            {
                FileDownloadName =
                    "MrCMS-ResourceData-" + CurrentRequestData.Now +
                    ".csv"
            };
        }

        public void Import(HttpPostedFileBase file)
        {
            Stream inputStream = file.InputStream;
            inputStream.Position = 0;
            var reader = new StreamReader(inputStream, Encoding.Default);
            string data = reader.ReadToEnd();
            string[] rows = data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var resourceData = new List<StringResourceData>();
            foreach (string row in rows.Skip(1)) //skip the header row
            {
                string[] columns = row.Split(new[] { "," }, StringSplitOptions.None);
                if (columns.Count() != 4)
                    continue;
                try
                {
                    string resourceKey = ReadData(columns[0]);

                    if (resourceData.All(x => x.Key != resourceKey))
                    {
                        resourceData.Add(new StringResourceData
                        {
                            Key = resourceKey,
                            Value = ReadData(columns[1]),
                            UICulture = ReadData(columns[2]),
                            SiteId = Convert.ToInt32(ReadData(columns[3]))
                        });
                    }

                }
                catch (Exception ex)
                {

                }
            }
            foreach (StringResourceData stringResourceData in resourceData)
            {
                string uiCulture = string.IsNullOrWhiteSpace(stringResourceData.UICulture)
                                       ? null
                                       : stringResourceData.UICulture;
                StringResource stringResource =
                    _provider.AllResources.FirstOrDefault(
                        resource =>
                        resource.Key == stringResourceData.Key && resource.Site.Id == stringResourceData.SiteId &&
                        resource.UICulture == uiCulture);
                if (stringResource != null)
                {
                    // we need to load the one out of the current session or else we'll have issues when saving
                    stringResource = _session.Get<StringResource>(stringResource.Id);
                    stringResource.Value = stringResourceData.Value;
                    _provider.Update(stringResource);
                }
                else
                {
                    _provider.Insert(new StringResource
                    {
                        Key = stringResourceData.Key,
                        Value = stringResourceData.Value,
                        UICulture = uiCulture,
                        Site = _session.Get<Site>(stringResourceData.SiteId)
                    });
                }
            }
        }

        private void AddToRow(List<string> row, string data)
        {
            row.Add(string.Format("{1}{0}{1}", (data ?? string.Empty).Replace(Quote, Quote + Quote), Quote));
        }

        private string ReadData(string value)
        {
            value = value ?? string.Empty;

            value = value.Trim('"');
            value = value.Replace("\"\"", "\"");
            return value;
        }

        public class StringResourceData
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public string UICulture { get; set; }
            public int SiteId { get; set; }
        }
    }
}