using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.AddOns.Pages;
using MrCMS.AddOns.Widgets;
using MrCMS.Entities.Widget;

namespace MrCMS.AddOns.DbConfiguration.Overrides
{
    public class WidgetOverides : IAutoMappingOverride<PlainTextWidget>
    {
        public void Override(AutoMapping<PlainTextWidget> mapping)
        {
            mapping.Map(x => x.Text).Length(4001); //4001 > == nvarcharmax
        }
    }
}
