using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Settings;
using MrCMS.Shortcodes.Forms;
using MrCMS.Website;
using ISession = NHibernate.ISession;

namespace MrCMS.Services
{
    public class FormPostingHandler : IFormPostingHandler
    {
        private readonly MailSettings _mailSettings;
        private readonly ISaveFormFileUpload _saveFormFileUpload;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGetWebpageForPath _getWebpageForPath;
        private readonly ISession _session;

        public const string GDPRConsent = nameof(GDPRConsent);

        public FormPostingHandler(MailSettings mailSettings, ISession session, ISaveFormFileUpload saveFormFileUpload,
            IHttpContextAccessor contextAccessor, IGetWebpageForPath getWebpageForPath)
        {
            _session = session;
            _saveFormFileUpload = saveFormFileUpload;
            _contextAccessor = contextAccessor;
            _getWebpageForPath = getWebpageForPath;
            _mailSettings = mailSettings;
        }

        public Form GetForm(int id)
        {
            return _session.Get<Form>(id);
        }

        public async Task<List<string>> SaveFormData(Form form, HttpRequest request)
        {
            var formProperties = form.FormProperties;
            var errors = new List<string>();
            if (!form.SendByEmailOnly)
            {
                var referer = new Uri(request?.Referer() ?? "/");
                var page = await _getWebpageForPath.GetWebpage(referer.AbsolutePath);
                if (page != null)
                    page = await _session.GetAsync<Webpage>(page.Id);
                var files = new List<IFormFile>();
                var formPosting = new FormPosting {Form = form, Webpage = page};
                await _session.TransactAsync(async session =>
                {
                    form.FormPostings.Add(formPosting);
                    await session.SaveOrUpdateAsync(formPosting);
                });
                await _session.TransactAsync(async (session) =>
                {
                    foreach (var formProperty in formProperties)
                    {
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
                                    files.Add(file);
                                    var value = await _saveFormFileUpload.SaveFile(form, formPosting, file);

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

                    if (form.ShowGDPRConsentBox)
                    {
                        var value = SanitizeValue(null, request.Form[GDPRConsent]);
                        if (string.IsNullOrWhiteSpace(value))
                            errors.Add("GDPR consent is required");
                    }

                    if (errors.Any())
                    {
                        await session.DeleteAsync(formPosting);
                    }
                    else
                    {
                        foreach (var value in formPosting.FormValues) await session.SaveAsync(value);

                        await SendFormMessages(form, formPosting, files);
                    }
                });
            }
            else
            {
                var values = new List<FormValue>();
                var files = new List<IFormFile>();
                foreach (var formProperty in formProperties)
                {
                    try
                    {
                        if (formProperty is FileUpload)
                        {
                            var file = request.Form.Files[formProperty.Name];

                            if (file == null && formProperty.Required)
                                throw new RequiredFieldException(
                                    $"No file was attached to the {formProperty.Name} field");

                            files.Add(file);
                        }
                        else
                        {
                            var value = SanitizeValue(formProperty, request.Form[formProperty.Name]);

                            if (string.IsNullOrWhiteSpace(value) && formProperty.Required)
                                throw new RequiredFieldException(
                                    $"No value was posted for the {formProperty.Name} field");

                            values.Add(new FormValue
                            {
                                Key = formProperty.Name,
                                Value = value,
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex.Message);
                    }
                }

                if (form.ShowGDPRConsentBox)
                {
                    var value = SanitizeValue(null, request.Form[GDPRConsent]);
                    if (string.IsNullOrWhiteSpace(value))
                        errors.Add("GDPR consent is required");
                }

                if (!errors.Any())
                {
                    await SendFormMessages(form, new FormPosting {FormValues = values}, files);
                }
            }

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

        private async Task SendFormMessages(Form form, FormPosting formPosting, List<IFormFile> files)
        {
            if (form.SendFormTo == null) return;

            var sendTo = form.SendFormTo.Split(',');
            if (sendTo.Any())
                await _session.TransactAsync(async session =>
                {
                    foreach (var email in sendTo)
                    {
                        var formFormMessage = form.FormMessage ?? "[form]";
                        var formMessage = ParseFormMessage(formFormMessage, form, formPosting);
                        var formTitle = form.FormEmailTitle ?? $"A new post to your form #{form.Name} has been made";

                        var attachments = new List<QueuedMessageAttachment>();
                        foreach (var file in files)
                        {
                            attachments.Add(new QueuedMessageAttachment
                            {
                                ContentType = file.ContentType,
                                Data = await GetData(file),
                                FileSize = file.Length,
                                FileName = file.FileName,
                            });
                        }

                        await session.SaveAsync(new QueuedMessage
                        {
                            Subject = formTitle,
                            Body = formMessage,
                            FromAddress = _mailSettings.SystemEmailAddress,
                            ToAddress = email,
                            IsHtml = true,
                            QueuedMessageAttachments = attachments
                        });
                    }
                });
        }

        private async Task<byte[]> GetData(IFormFile formFile)
        {
            await using var memoryStream = new MemoryStream();
            var readStream = formFile.OpenReadStream();
            await readStream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        private static string ParseFormMessage(string formMessage, Form form, FormPosting formPosting)
        {
            var formRegex = new Regex(@"\[form\]");
            
            if (!formRegex.IsMatch(formMessage))
            {
                formMessage = "[form]" + formMessage;
            }

            return formRegex.Replace(formMessage, match =>
            {
                var list = new TagBuilder("ul");

                foreach (var formValue in formPosting.FormValues)
                {
                    var listItem = new TagBuilder("li");

                    var title = new TagBuilder("b");
                    title.InnerHtml.AppendHtml(formValue.Key + ":");
                    listItem.InnerHtml.AppendHtml(title.GetString() + " " +
                                                  formValue.GetMessageValue());

                    list.InnerHtml.AppendHtml(listItem.GetString());
                }

                return list.GetString();
            });
        }
    }
}