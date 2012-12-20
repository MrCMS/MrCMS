namespace MrCMS.Entities.Documents.Web
{
    public abstract class ProcessPage : Webpage
    {
    }
    public abstract class ProcessPage<T> : ProcessPage where T : ProcessPage
    {
        public virtual T RedirectTo { get; set; }
    }
}