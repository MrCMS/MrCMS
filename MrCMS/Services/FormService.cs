using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Tasks;
using NHibernate;

namespace MrCMS.Services
{
    public class FormService : IFormService
    {
        private readonly ISession _session;
        private readonly IDocumentService _documentService;
        private readonly SiteSettings _siteSettings;
        private readonly MailSettings _mailSettings;

        public FormService(ISession session, IDocumentService documentService, SiteSettings siteSettings, MailSettings mailSettings)
        {
            _session = session;
            _documentService = documentService;
            _siteSettings = siteSettings;
            _mailSettings = mailSettings;
        }

        public string GetFormStructure(Webpage webpage)
        {
            return webpage == null
                       ? Newtonsoft.Json.JsonConvert.SerializeObject(new object())
                       : webpage.FormData ?? Newtonsoft.Json.JsonConvert.SerializeObject(new object());
        }

        public void SaveFormStructure(Webpage webpage, string data)
        {
            if (webpage == null) return;
            webpage.FormData = data;
            _documentService.SaveDocument(webpage);
        }

        public void SaveFormData(Webpage webpage, FormCollection formCollection)
        {
            _session.Transact(session =>
                                                   {
                                                       if (webpage == null) return;
                                                       var formPosting = new FormPosting
                                                                             {
                                                                                 Webpage = webpage,
                                                                                 FormValues = new List<FormValue>()
                                                                             };
                                                       formCollection.AllKeys.ForEach(s =>
                                                                                          {
                                                                                              var formValue = new FormValue
                                                                                                                  {
                                                                                                                      Key = s,
                                                                                                                      Value = formCollection[s],
                                                                                                                      FormPosting = formPosting,
                                                                                                                  };
                                                                                              formPosting.FormValues.Add(formValue);
                                                                                              session.SaveOrUpdate(formValue);
                                                                                          });

                                                       webpage.FormPostings.Add(formPosting);
                                                       session.SaveOrUpdate(formPosting);

                                                       SendFormMessages(webpage, formPosting);
                                                   });
        }

        private void SendFormMessages(Webpage webpage, FormPosting formPosting)
        {
            if (webpage.SendFormTo == null) return;

            var sendTo = webpage.SendFormTo.Split(',');
            if (sendTo.Any())
            {
                _session.Transact(session =>
                                      {
                                          foreach (var email in sendTo)
                                          {
                                              var formMessage = ParseFormMessage(webpage.FormMessage, webpage,
                                                                                 formPosting);
                                              var formTitle = ParseFormMessage(webpage.FormEmailTitle, webpage,
                                                                               formPosting);

                                              session.SaveOrUpdate(new QueuedMessage
                                                                       {
                                                                           Subject = formTitle,
                                                                           Body = formMessage,
                                                                           FromAddress = _mailSettings.SystemEmailAddress,
                                                                           ToAddress = email,
                                                                           IsHtml = true
                                                                       });
                                          }

                                          TaskExecutor.ExecuteLater(new SendQueuedMessagesTask(_mailSettings,
                                                                                               _siteSettings));
                                      });
            }
        }

        private static string ParseFormMessage(string formMessage, Webpage webpage, FormPosting formPosting)
        {

            var formRegex = new Regex(@"\[form\]");
            var pageRegex = new Regex(@"{{page.(.*)}}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var messageRegex = new Regex(@"{{(.*)}}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            formMessage = formRegex.Replace(formMessage, match =>
                                                             {
                                                                 var list = new TagBuilder("ul");

                                                                 foreach (var formValue in formPosting.FormValues)
                                                                 {
                                                                     var listItem = new TagBuilder("li");

                                                                     var title = new TagBuilder("b");
                                                                     title.InnerHtml += formValue.Key + ":";
                                                                     listItem.InnerHtml += title.ToString() + " " +
                                                                                           formValue.Value;

                                                                     list.InnerHtml += listItem.ToString();
                                                                 }

                                                                 return list.ToString();
                                                             });

            formMessage = pageRegex.Replace(formMessage, match =>
                                                             {
                                                                 var propertyInfo =
                                                                     typeof(Webpage).GetProperties().FirstOrDefault(
                                                                         info =>
                                                                         info.Name.Equals(match.Value.Replace("{", "").Replace("}", "").Replace("page.", ""),
                                                                                          StringComparison.OrdinalIgnoreCase));

                                                                 return propertyInfo == null
                                                                            ? string.Empty
                                                                            : propertyInfo.GetValue(webpage,
                                                                                                    null).
                                                                                           ToString();
                                                             });
            return messageRegex.Replace(formMessage, match =>
                                                         {
                                                             var formValue =
                                                                 formPosting.FormValues.FirstOrDefault(
                                                                     value =>
                                                                     value.Key.Equals(
                                                                         match.Value.Replace("{", "").Replace("}", ""),
                                                                         StringComparison.
                                                                             OrdinalIgnoreCase));
                                                             return formValue == null
                                                                        ? string.Empty
                                                                        : formValue.Value;
                                                         });
        }

        public FormPosting GetFormPosting(int id)
        {
            return _session.Get<FormPosting>(id);
        }

        public PostingsModel GetFormPostings(Webpage webpage, int page, string search)
        {
            IEnumerable<FormPosting> formPostings = webpage.FormPostings.OrderByDescending(posting => posting.CreatedOn);

            if (!string.IsNullOrWhiteSpace(search))
            {
                formPostings =
                    formPostings.Where(
                        posting =>
                        posting.FormValues.Any(value => value.Value.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            return new PostingsModel(new PagedList<FormPosting>(formPostings, page, 10), webpage.Id);
        }
    }
}