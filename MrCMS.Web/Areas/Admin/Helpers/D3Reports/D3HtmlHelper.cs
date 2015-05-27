using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Web.Areas.Admin.Models.UserSubscriptionReports;
using System.Linq.Expressions;

namespace MrCMS.Web.Areas.Admin.Helpers.D3Reports
{
    public static class D3HtmlHelper
    {
       
        /// <summary>
        /// Returns a  BootStrap DatePicker element that is represented by the specified expression.
        /// </summary>
        /// <typeparam name="TModel">Template Model</typeparam>
        /// <typeparam name="TProperty">Template Model Property</typeparam>
        /// <param name="htmlHelper">htmlHelper</param>
        /// <param name="expression"></param>
        /// <param name="className">class name for DatePicker</param>
        /// /// <param name="format">Date format for DatePicker</param>
        /// <param name="labelforTooltip">label for tooltip</param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapDatePickerFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string className = "",string format="dd/mm/yyyy", string labelforTooltip = "")
        {
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class=\"form-group\">");
            if (!string.IsNullOrWhiteSpace(labelforTooltip))
            {
                sb.AppendLine(htmlHelper.LabelFor(expression, new { @data_toggle = "tooltip", @data_placement = "top", @title = labelforTooltip }).ToString());
            }
            else
            {
                sb.AppendLine(htmlHelper.LabelFor(expression).ToString());
            }
            sb.AppendLine("<div class=\"input-group date date-picker\" data-date-format=\"" + format + "\">");
            string placeholder = HttpUtility.HtmlDecode(htmlHelper.DisplayNameFor(expression).ToHtmlString());
            if (!string.IsNullOrWhiteSpace(className))
            {
                className = " " + className;
            }
            sb.AppendLine(htmlHelper.TextBoxFor(expression, new { @class = "form-control" + className, @placeholder = placeholder}).ToString());
            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ToString());
            sb.AppendLine("<span class=\"input-group-addon btn\"><i class=\"fa fa-calendar\"></i></span></div></div>");
            return new MvcHtmlString(sb.ToString());
        }

    }
}