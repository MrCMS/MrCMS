using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Documents.Web.FormProperties;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Documents.Web
{
    public class Form : SiteEntity
    {
        public Form()
        {
            FormPostings = new List<FormPosting>();
            FormProperties = new List<FormProperty>();
        }
        [Required]
        public virtual string Name { get; set; }

        //forms
        [DisplayName("Form Submitted Message")]
        //[AllowHtml]
        [StringLength(500, ErrorMessage = "Form submitted messsage cannot be longer than 500 characters."), IsDBLength]
        public virtual string FormSubmittedMessage { get; set; }

        [DisplayName("Form Success Redirect")]
        public virtual string FormRedirectUrl { get; set; }

        [DisplayName("Subject")]
        [StringLength(250, ErrorMessage = "Subject cannot be longer than 250 characters."), IsDBLength]
        public virtual string FormEmailTitle { get; set; }

        [DisplayName("Send Form To")]
        [StringLength(500, ErrorMessage = "Send to cannot be longer than 500 characters."), IsDBLength]
        public virtual string SendFormTo { get; set; }

        [DisplayName("Form Email Message")]
        public virtual string FormMessage { get; set; }

        [StringLength(100), IsDBLength]
        [DisplayName("Submit Button Css Class")]
        public virtual string SubmitButtonCssClass { get; set; }

        [StringLength(100), IsDBLength]
        [DisplayName("Submit button custom text")]
        public virtual string SubmitButtonText { get; set; }


        public virtual IList<FormProperty> FormProperties { get; set; }
        public virtual IList<FormPosting> FormPostings { get; set; }


        public virtual string FormDesign { get; set; }
    }
}