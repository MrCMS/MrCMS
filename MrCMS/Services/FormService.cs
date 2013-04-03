using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Shortcodes.Forms;
using MrCMS.Tasks;
using NHibernate;

namespace MrCMS.Services
{
    public class FormService : IFormService
    {
        private readonly ISession _session;
        private readonly IDocumentService _documentService;
        private readonly IFileService _fileService;
        private readonly SiteSettings _siteSettings;
        private readonly MailSettings _mailSettings;

        public FormService(ISession session, IDocumentService documentService, IFileService fileService, SiteSettings siteSettings, MailSettings mailSettings)
        {
            _session = session;
            _documentService = documentService;
            _fileService = fileService;
            _siteSettings = siteSettings;
            _mailSettings = mailSettings;
        }

        public List<string> SaveFormData(Webpage webpage, HttpRequestBase request)
        {
            var formProperties = webpage.FormProperties;

            var formPosting = new FormPosting { Webpage = webpage };
            _session.Transact(session =>
                                  {
                                      webpage.FormPostings.Add(formPosting);
                                      session.SaveOrUpdate(formPosting);
                                  });
            var errors = new List<string>();
            _session.Transact(session =>
                                  {
                                      foreach (var formProperty in formProperties)
                                      {
                                          try
                                          {
                                              if (formProperty is FileUpload)
                                              {
                                                  var file = request.Files[formProperty.Name];

                                                  if (file == null && formProperty.Required)
                                                      throw new RequiredFieldException("No file was attached to the " +
                                                                                       formProperty.Name + " field");

                                                  if (file != null && !string.IsNullOrWhiteSpace(file.FileName))
                                                  {
                                                      var value = SaveFile(webpage, formPosting, file);

                                                      formPosting.FormValues.Add(new FormValue
                                                                                     {
                                                                                         Key = formProperty.Name,
                                                                                         Value = value,
                                                                                         IsFile = true,
                                                                                         FormPosting = formPosting
                                                                                     });
                                                  }
                                              }
                                              else
                                              {
                                                  var value = SanitizeValue(formProperty, request.Form[formProperty.Name]);

                                                  if (string.IsNullOrWhiteSpace(value) && formProperty.Required)
                                                      throw new RequiredFieldException("No value was posted for the " +
                                                                                       formProperty.Name + " field");

                                                  formPosting.FormValues.Add(new FormValue
                                                                                 {
                                                                                     Key = formProperty.Name,
                                                                                     Value = value,
                                                                                     FormPosting = formPosting
                                                                                 });
                                              }
                                          }
                                          catch (Exception ex)
                                          {
                                              errors.Add(ex.Message);
                                          }
                                      }

                                      if (errors.Any())
                                      {
                                          session.Delete(formPosting);
                                      }
                                      else
                                      {
                                          formPosting.FormValues.ForEach(value => session.Save(value));

                                          SendFormMessages(webpage, formPosting);
                                      }
                                  });
            return errors;
        }

        private string SanitizeValue(FormProperty formProperty, string value)
        {
            if (formProperty is CheckboxList)
            {
                if (value != null)
                {
                    var list = value.Split(',').ToList();
                    list.Remove(CheckBoxListRenderer.CbHiddenValue);
                    return !list.Any() ? null : string.Join(",", list);
                }
                return value;
            }
            return value;
        }

        private string SaveFile(Webpage webpage, FormPosting formPosting, HttpPostedFileBase file)
        {
            var mediaCategory = _documentService.GetDocumentByUrl<MediaCategory>("file-uploads") ??
                                CreateFileUploadMediaCategory();

            var result = _fileService.AddFile(file.InputStream, webpage.Id + "-" + formPosting.Id + "-" + file.FileName, file.ContentType, file.ContentLength, mediaCategory);

            return result.Path;
        }

        private MediaCategory CreateFileUploadMediaCategory()
        {
            var mediaCategory = new MediaCategory { UrlSegment = "file-uploads", Name = "File Uploads" };
            _documentService.AddDocument(mediaCategory);
            return mediaCategory;
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
                                                                                                     Value =
                                                                                                         formCollection[
                                                                                                             s],
                                                                                                     FormPosting =
                                                                                                         formPosting,
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

        public void AddFormProperty(FormProperty property)
        {
            _session.Transact(session => session.Save(property));
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
            var formProperty = formListOption.FormProperty;
            if (formProperty != null)
                formProperty.Options.Add(formListOption);
            _session.Transact(session => session.Save(formListOption));
        }

        public void UpdateFormListOption(FormListOption formListOption)
        {
            _session.Transact(session => session.Update(formListOption));
        }

        public void DeleteFormListOption(FormListOption formListOption)
        {
            _session.Transact(session => session.Delete(formListOption));
        }
    }

    public class RequiredFieldException : Exception
    {
        public RequiredFieldException(string message)
            : base(message)
        {

        }
    }
}