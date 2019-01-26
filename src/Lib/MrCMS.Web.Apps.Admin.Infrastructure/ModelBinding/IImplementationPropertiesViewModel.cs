using MrCMS.Entities;

namespace MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding
{
    /// <summary>
    /// Marker interface for model-binder properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUpdatePropertiesViewModel<T> where T : SystemEntity, new()
    {

    }

    /// <summary>
    /// Marker interface for model-binder properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAddPropertiesViewModel<T> where T : SystemEntity, new()
    {

    }
}