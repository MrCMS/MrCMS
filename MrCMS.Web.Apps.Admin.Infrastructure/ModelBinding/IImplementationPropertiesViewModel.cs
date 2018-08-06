using MrCMS.Entities;

namespace MrCMS.Web.Apps.Admin.Models.Tabs
{
    /// <summary>
    /// Marker interface for model-binder properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IImplementationPropertiesViewModel<T> where T : SystemEntity, new()
    {

    }
}