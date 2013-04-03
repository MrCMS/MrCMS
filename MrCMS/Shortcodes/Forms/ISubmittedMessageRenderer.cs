using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Shortcodes.Forms
{
    public interface ISubmittedMessageRenderer
    {
        TagBuilder AppendSubmittedMessage(Webpage webpage, FormSubmittedStatus submittedStatus);
    }
}