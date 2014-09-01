using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class FormAdminService : IFormAdminService
    {
        private readonly ISession _session;

        public FormAdminService(ISession session)
        {
            _session = session;
        }

        public void ClearFormData(Webpage webpage)
        {
            _session.Transact(session =>
            {
                webpage.FormPostings.ForEach(session.Delete);
                webpage.FormPostings.Clear();
            });
        }

        public byte[] ExportFormData(Webpage webpage)
        {
            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            using (var w = new CsvWriter(sw))
            {
                foreach (string header in GetHeadersForExport(webpage))
                {
                    w.WriteField(header);
                }
                w.NextRecord();

                foreach (var item in GetFormDataForExport(webpage))
                {
                    foreach (string value in item.Value)
                    {
                        w.WriteField(value);
                    }
                    w.NextRecord();
                }

                sw.Flush();
                byte[] file = ms.ToArray();
                sw.Close();

                return file;
            }
        }

        public void DeletePosting(FormPosting posting)
        {
            posting.Webpage.FormPostings.Remove(posting);
            _session.Transact(session => session.Delete(posting));
        }

        public void AddFormProperty(FormProperty property)
        {
            _session.Transact(session =>
            {
                if (property.Webpage.FormProperties != null)
                    property.DisplayOrder = property.Webpage.FormProperties.Count;

                session.Save(property);
            });
        }

        public void SaveFormProperty(FormProperty property)
        {
            _session.Transact(session => session.Update(property));
        }

        public void DeleteFormProperty(FormProperty property)
        {
            _session.Transact(session => session.Delete(property));
        }

        public void SaveFormListOption(FormListOption formListOption)
        {
            FormPropertyWithOptions formProperty = formListOption.FormProperty;
            if (formProperty != null)
                formProperty.Options.Add(formListOption);
            _session.Transact(session =>
            {
                formListOption.OnSaving(session);
                session.Save(formListOption);
            });
        }

        public void UpdateFormListOption(FormListOption formListOption)
        {
            _session.Transact(session =>
            {
                formListOption.OnSaving(session);
                session.Update(formListOption);
            });
        }

        public void DeleteFormListOption(FormListOption formListOption)
        {
            _session.Transact(session => session.Delete(formListOption));
        }

        public void SetOrders(List<SortItem> items)
        {
            _session.Transact(session => items.ForEach(item =>
            {
                var formItem = session.Get<FormProperty>(item.Id);
                formItem.DisplayOrder = item.Order;
                session.Update(formItem);
            }));
        }

        public PostingsModel GetFormPostings(Webpage webpage, int page, string search)
        {
            FormPosting posting = null;
            var query =
                _session.QueryOver(() => posting).Where(() => posting.Webpage.Id == webpage.Id);
            if (!string.IsNullOrWhiteSpace(search))
            {
                query =
                    query.WithSubquery.WhereExists(
                        QueryOver.Of<FormValue>()
                            .Where(
                                value =>
                                    value.FormPosting.Id == posting.Id &&
                                    value.Value.IsInsensitiveLike(search, MatchMode.Anywhere)).Select(value => value.Id));
            }

            IPagedList<FormPosting> formPostings = query.OrderBy(() => posting.CreatedOn).Desc.Paged(page);

            return new PostingsModel(formPostings, webpage.Id);
        }

        private static IEnumerable<string> GetHeadersForExport(Webpage webpage)
        {
            var headers = new List<string>();
            foreach (FormPosting posting in webpage.FormPostings)
            {
                headers.AddRange(posting.FormValues.Select(x => x.Key).Distinct());
            }
            return headers.Distinct().ToList();
        }

        private static Dictionary<int, List<string>> GetFormDataForExport(Webpage webpage)
        {
            var items = new Dictionary<int, List<string>>();
            for (int i = 0; i < webpage.FormPostings.Count; i++)
            {
                FormPosting posting = webpage.FormPostings[i];
                items.Add(i, new List<string>());
                foreach (
                    FormValue value in
                        GetHeadersForExport(webpage)
                            .SelectMany(header => posting.FormValues.Where(x => x.Key == header)))
                {
                    if (!value.IsFile)
                        items[i].Add(value.Value);
                    else
                        items[i].Add("http://" + CurrentRequestData.CurrentSite.BaseUrl + value.Value);
                }
            }
            return items.OrderByDescending(x => x.Value.Count).ToDictionary(pair => pair.Key, pair => pair.Value);
        }


        public FormPosting GetFormPosting(int id)
        {
            return _session.Get<FormPosting>(id);
        }
    }
}