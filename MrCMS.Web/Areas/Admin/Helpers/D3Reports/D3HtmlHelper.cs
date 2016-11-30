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
        /// Generate Axis Graph with points bubble based on model and property
        /// </summary>
        /// <typeparam name="TModel">Template Model</typeparam>
        /// <typeparam name="TProperty">Template Model Property</typeparam>
        /// <param name="htmlHelper">htmlHelper</param>
        /// <param name="divID">Parent Div id for SVG</param>
        /// <param name="tooltipX">Set ToolTip X Values Label</param>
        /// <param name="tooltipY">Set ToolTip Y Values Label</param>
        /// <param name="width">Width of SVG</param>
        /// <param name="height">Height of SVG</param>
        /// <param name="margin">Margin for SVG</param>
        /// <returns>D3 Graph Html String with D3 Scripts</returns>
        /// 
       

        public static MvcHtmlString D3GraphAxisComponentFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, TProperty entity, string divID = "graph",string tooltipX="Subscribed On ",string tooltipY= "No of Users ", int width = 900, int height = 400, int margin = 50)
        {

            StringBuilder builder = new StringBuilder();
            ID3AxisComponent component = entity as ID3AxisComponent;

            string textOnGraph = component.Label,
                    xComponent = component.XComponent,
                    yComponent = component.YComponent,
                    data = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(component.JsonObject);

            builder.AppendLine(@"<script src='/Areas/Admin/Content/Scripts/lib/d3/d3.min.js'></script>");
            builder.AppendLine(@"<script src='/Areas/Admin/Content/Scripts/lib/d3/d3.tip.js'></script>");
            builder.AppendLine(string.Format(JavaScript, data, divID, margin, width, height, tooltipX, tooltipY, yComponent, xComponent, textOnGraph));

            return new MvcHtmlString(builder.ToString());
        }
       
        public static string JavaScript
        {
            get
            {
                return @"<script>
                            var ijson = '{0}';
                            var idata = JSON.parse(ijson);
                            var icontainer = 'div#{1}';

                            var AxisComponent = function () {{
                                var container = icontainer,
                                 margin = {{ top:{2} , right: {2}, bottom: {2}, left: {2} }},
                                 width = {3} - margin.left - margin.right,
                                 height = {4} - margin.top - margin.bottom,
                                 parse = d3.time.format('%b %Y').parse,
                                 formatTime = d3.time.format('%B %Y'),
                                 data = idata,
                                 svg = null,

                                 x = d3.time.scale().range([0, width]),
                                 y = d3.scale.linear().range([height, 0]),

                                 xAxis = d3.svg.axis().scale(x).ticks(5).tickSize(-height).tickSubdivide(true),
                                 yAxis = d3.svg.axis().scale(y).ticks(5).orient('right');

                                 area = d3.svg.area()
                                .interpolate('monotone')
                                .x(function (d) {{ return x(d.{8}); }})
                                .y0(height)
                                .y1(function (d) {{ return y(d.{7}); }});

                                line = d3.svg.line()
                                .interpolate('monotone')
                                .x(function (d) {{ return x(d.{8}); }})
                                .y(function (d) {{ return y(d.{7}); }}),

                                tip = d3.tip()
                                   .attr('class', 'd3-tip')
                                   .offset([-10, 0])
                                   .html(function (d) {{
                                       return '<strong>{5}</strong> <span style=color:red>' + formatTime(new Date(d.{8})) + '</span><br><strong>{6}</strong> <span style=color:red>' + d.{7} + '</span>';
                                   }});

                                this.DataInit = function () {{
                                    data.forEach(type);
                                    data.sort(function (a, b) {{ return d3.ascending(a.{8}, b.{8}); }});
                                }};

                                this.DomainInit = function () {{
                                    x.domain([data[0].{8}, data[data.length - 1].{8}]);
                                    y.domain([0, d3.max(data, function (d) {{ return d.{7}; }})]).nice();
                                }};
        

                                this.ToolTipInit = function (d) {{
                                    svg.selectAll('.dot').remove();
                                    svg.selectAll('.dot')
                                   .data(d)
                                   .enter().append('circle')
                                   .attr('class', 'dot')
                                   .attr('cx', line.x())
                                   .attr('cy', line.y())
                                   .attr('r', 7)
                                   .attr('stroke', '#009900')
                                   .attr('fill', 'purple')
                                   .on('mouseover', this.tip.show)
                                   .on('mouseout',  this.tip.hide);
                                }};

                                this.SVGInit = function () {{
                                         svg = d3.select('div#graph').append('svg')
                                            .attr('width', width + margin.left + margin.right)
                                            .attr('height', height + margin.top + margin.bottom)
                                            .append('g')
                                            .attr('transform', 'translate(' + margin.left + ',' + margin.top + ')')
                                            .on('click', click);

                                    svg.append('clipPath')
                                                .attr('id', 'clip')
                                                .append('rect')
                                                .attr('width', width)
                                                .attr('height', height);

                                    svg.append('path')
                                        .attr('class', 'area')
                                        .attr('clip-path', 'url(#clip)')
                                        .attr('d', area(data));

                                    svg.append('g')
                                        .attr('class', 'x axis')
                                        .attr('transform', 'translate(0,' + height + ')')
                                        .call(xAxis);

                                    svg.append('g')
                                        .attr('class', 'y axis')
                                        .attr('transform', 'translate(' + width + ',0)')
                                        .call(yAxis);

                                    svg.append('path')
                                        .attr('class', 'line')
                                        .attr('clip-path', 'url(#clip)')
                                        .attr('d', line(data));

                                    svg.append('text')
                                        .attr('x', width - 6)
                                        .attr('y', height - 6)
                                        .style('text-anchor', 'end')
                                        .text('{9}');

           
                                    ToolTipInit(data);
                                    svg.call(this.tip);
           
                                }};

                                this.click = function () {{
                                    var n = data.length - 1,
                                    i = Math.floor(Math.random() * n / 2),
                                    j = i + Math.floor(Math.random() * n / 2) + 1;

                                    x.domain([data[i].{8}, data[j].{8}]);
                                    var newData = data.slice(i, j + 1);
                                    var t = svg.transition().duration(500);
                                    t.select('.x.axis').call(xAxis);
                                    t.select('.area').attr('d', area(data));
                                    t.select('.line').attr('d', line(data));
           
                                   ToolTipInit(newData);
                                }};

                                this.type = function (d) {{
                                    d.{8} = parse(d.{8});
                                    d.{7} = +d.{7};
                                    return d;
                                }};

                                this.Init = function() {{
                                    this.DataInit();
                                    this.DomainInit();
                                    this.SVGInit();
                                }};
                                Init();
                            }}();

                        </script>";
            }
        }

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
            sb.AppendLine("<div class=\"input-group date date-picker\" data-date-format=\"" + format + "\" >");
            string placeholder = HttpUtility.HtmlDecode(htmlHelper.DisplayNameFor(expression).ToHtmlString());
            if (!string.IsNullOrWhiteSpace(className))
            {
                className = " " + className;
            }
            sb.AppendLine(htmlHelper.TextBoxFor(expression, new { @class = "form-control" + className, @placeholder = placeholder }).ToString());
            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ToString());
            sb.AppendLine("<span class=\"input-group-addon btn\"><i class=\"fa fa-calendar\"></i></span></div></div>");
            return new MvcHtmlString(sb.ToString());
        }

    }
}