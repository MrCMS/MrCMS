namespace MrCMS.Web.Areas.Admin.Models.Forms
{
    public class FormMessageTabViewModel
    {
        public int Id { get; set; }
        public string FormSubmittedMessage { get; set; }
        public string FormRedirectUrl { get; set; }
        public string SubmitButtonText { get; set; }
        public string SubmitButtonCssClass { get; set; }
        public string FormEmailTitle { get; set; }
        public string SendFormTo { get; set; }
        public string FormMessage { get; set; }
    }
}