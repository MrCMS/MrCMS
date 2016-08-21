using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class FormAdminService : IFormAdminService
    {
        private readonly IRepository<FormPosting> _formPostingRepository;
        private readonly IRepository<FormProperty> _formPropertyRepository;
        private readonly IRepository<FormListOption> _formListOptionRepository;
        private readonly IRepository<FormValue> _formValueRepository;

        public FormAdminService(IRepository<FormPosting>  formPostingRepository, IRepository<FormProperty> formPropertyRepository,
            IRepository<FormListOption> formListOptionRepository, IRepository<FormValue> formValueRepository)
        {
            _formPostingRepository = formPostingRepository;
            _formPropertyRepository = formPropertyRepository;
            _formListOptionRepository = formListOptionRepository;
            _formValueRepository = formValueRepository;
        }

        public void ClearFormData(Webpage webpage)
        {
            _formPostingRepository.Transact(repository =>
            {
                var webpageFormPostings = webpage.FormPostings.ToList();
                webpage.FormPostings.Clear();
                webpageFormPostings.ForEach(repository.Delete);
            });
        }

        public byte[] ExportFormData(Webpage webpage)
        {
            var stringBuilder = new StringBuilder();

            var headers = GetHeadersForExport(webpage).ToList();
            stringBuilder.AppendLine(string.Join(",", headers.Select(FormatField)));

            var formDataForExport = GetFormDataForExport(webpage);
            foreach (var data in formDataForExport)
            {
                stringBuilder.AppendLine(string.Join(",", data.Value.Select(FormatField)));
            }

            return Encoding.Default.GetBytes(stringBuilder.ToString());
        }

        public void DeletePosting(FormPosting posting)
        {
            posting.Webpage.FormPostings.Remove(posting);
            _formPostingRepository.Delete(posting);
        }

        public void AddFormProperty(FormProperty property)
        {
            _formPropertyRepository.Transact(repository =>
            {
                if (property.Webpage.FormProperties != null)
                    property.DisplayOrder = property.Webpage.FormProperties.Count;

                repository.Add(property);
            });
        }

        public void SaveFormProperty(FormProperty property)
        {
            _formPropertyRepository.Update(property);
        }

        public void DeleteFormProperty(FormProperty property)
        {
            property.Webpage.FormProperties.Remove(property);
            _formPropertyRepository.Delete(property);
        }

        public void SaveFormListOption(FormListOption formListOption)
        {
            FormPropertyWithOptions formProperty = formListOption.FormProperty;
            if (formProperty != null)
                formProperty.Options.Add(formListOption);
            _formListOptionRepository.Transact(repository =>
            {
                formListOption.OnSaving(repository);
                repository.Add(formListOption);
            });
        }

        public void UpdateFormListOption(FormListOption formListOption)
        {
            _formListOptionRepository.Transact(repository =>
            {
                formListOption.OnSaving(repository);
                repository.Update(formListOption);
            });
        }

        public void DeleteFormListOption(FormListOption formListOption)
        {
            _formListOptionRepository.Delete(formListOption);
        }

        public void SetOrders(List<SortItem> items)
        {
            _formPropertyRepository.Transact(session => items.ForEach(item =>
            {
                var formItem = session.Get(item.Id);
                formItem.DisplayOrder = item.Order;
                session.Update(formItem);
            }));
        }

        public PostingsModel GetFormPostings(Webpage webpage, int page, string search)
        {
            var query =
                _formPostingRepository.Query().Where(posting => posting.Webpage.Id == webpage.Id);
            if (!string.IsNullOrWhiteSpace(search))
            {
                query =
                    query.Where(posting=>
                        _formValueRepository.Query()
                            .Where(
                                value =>
                                    value.Value.Like($"%{search}%")).Select(value => value.FormPosting.Id).Contains(posting.Id));
            }

            IPagedList<FormPosting> formPostings = query.OrderByDescending(posting => posting.CreatedOn).Paged(page);

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


        private string FormatField(string data)
        {
            return string.Format("{1}{0}{1}", (data ?? string.Empty).Replace(Quote, Quote + Quote), Quote);
        }
        public const string Quote = "\"";
    }
}