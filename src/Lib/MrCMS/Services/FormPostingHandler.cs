using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Settings;
using MrCMS.Shortcodes.Forms;
using ISession = NHibernate.ISession;

namespace MrCMS.Services
{
    public class FormPostingHandler : IFormPostingHandler
    {
        private readonly MailSettings _mailSettings;
        private readonly ISaveFormFileUpload _saveFormFileUpload;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ISession _session;

        public FormPostingHandler(MailSettings mailSettings, ISession session, ISaveFormFileUpload saveFormFileUpload, IHttpContextAccessor contextAccessor)
        {
            _session = session;
            _saveFormFileUpload = saveFormFileUpload;
            _contextAccessor = contextAccessor;
            _mailSettings = mailSettings;
        }

        public Form GetForm(int id)
        {
            return _session.Get<Form>(id);
        }

        public List<string> SaveFormData(Form form, HttpRequest request)
        {
            var formProperties = form.FormProperties;

            var formPosting = new FormPosting {Form = form};
            _session.Transact(session =>
            {
                form.FormPostings.Add(formPosting);
                session.SaveOrUpdate(formPosting);
            });
            var errors = new List<string>();
            _session.Transact(session =>
            {
                foreach (var formProperty in formProperties)
                    try
                    {
                        if (formProperty is FileUpload)
                        {
                            var file = request.Form.Files[formProperty.Name];

                            if (file == null && formProperty.Required)
                                throw new RequiredFieldException("No file was attached to the " +
                                                                 formProperty.Name + " field");

                            if (file != null && !string.IsNullOrWhiteSpace(file.FileName))
                            {
                                var value = _saveFormFileUpload.SaveFile(form, formPosting, file);

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

                if (errors.Any())
                {
                    session.Delete(formPosting);
                }
                else
                {
                    foreach (var value in formPosting.FormValues) session.Save(value);

                    SendFormMessages(form, formPosting);
                }
            });
            return errors;
        }

        public string GetRefererUrl()
        {
            return _contextAccessor.HttpContext?.Request?.Referer();
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

        private void SendFormMessages(Form form, FormPosting formPosting)
        {
            if (form.SendFormTo == null) return;

            var sendTo = form.SendFormTo.Split(',');
            if (sendTo.Any())
                _session.Transact(session =>
                {
                    foreach (var email in sendTo)
                    {
                        var formMessage = ParseFormMessage(form.FormMessage, form,
                            formPosting);
                        var formTitle = ParseFormMessage(form.FormEmailTitle, form,
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
                });
        }

        private static string ParseFormMessage(string formMessage, Form form, FormPosting formPosting)
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
                    title.InnerHtml.AppendHtml( formValue.Key + ":");
                    listItem.InnerHtml.AppendHtml(title.GetString() + " " +
                                          formValue.GetMessageValue());

                    list.InnerHtml.AppendHtml(listItem.GetString());
                }

                return list.GetString();
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
                    : propertyInfo.GetValue(form, null).ToString();
            });
            return messageRegex.Replace(formMessage, match =>
            {
                var formValue =
                    formPosting.FormValues.FirstOrDefault(
                        value =>
                            value.Key.Equals(
                                match.Value.Replace("{", "").Replace("}", ""),
                                StringComparison.OrdinalIgnoreCase));
                return formValue == null
                    ? string.Empty
                    : formValue.GetMessageValue();
            });
        }
    }
}