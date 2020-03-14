using System.Threading.Tasks;
using MrCMS.Entities.Widget;

namespace MrCMS.Services.Widgets
{
    public sealed class DefaultGetWidgetModel : GetWidgetModelBase
    {
        private DefaultGetWidgetModel()
        {

        }

        static DefaultGetWidgetModel()
        {
            Instance = new DefaultGetWidgetModel();
        }

        public readonly static DefaultGetWidgetModel Instance;
        public override Task<object> GetModel(Widget widget)
        {
            return Task.FromResult<object>(widget);
        }
    }
}