namespace MrCMS.Batching
{
    // TODO: model binder
    //public class BatchRunGuidModelBinder : MrCMSDefaultModelBinder
    //{
    //    public BatchRunGuidModelBinder(IKernel kernel)
    //        : base(kernel)
    //    {
    //    }

    //    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    //    {
    //        var id = Convert.ToString(controllerContext.RouteData.Values["id"]);
    //        Guid guid;
    //        if (Guid.TryParse(id, out guid))
    //        {
    //            return Session.Query<BatchRun>().FirstOrDefault(x => x.Guid == guid);
    //        }
    //        return null;
    //    }
    //}
}