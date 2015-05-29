(function ($, d3) {
    'use strict';

    $(function () {
        $('[data-d3-graph]').each(function (index, element) {
            var $element = $(element);
            $.get($element.data('d3-graph'), function(data) {
                lineGraph(element, data, $element.data('d3-graph-label'),
                    $element.data('d3-x-axis-label'),
                    $element.data('d3-y-axis-label')
                );
            });
        });
    });

    function lineGraph(element, graphData, graphLabel, tooltipX, tooltipY) {
        var container = element,
            data = graphData,
            margin = { top: 50, right: 50, bottom: 50, left: 50 },
            width = 900 - margin.left - margin.right,
            height = 450 - margin.top - margin.bottom,
            parse = d3.time.format('%b %Y').parse,
            formatTime = d3.time.format('%B %Y'),

            svg = null,

            x = d3.time.scale().range([0, width]),
            y = d3.scale.linear().range([height, 0]),

            xAxis = d3.svg.axis().scale(x).ticks(5).tickSize(-height).tickSubdivide(true),
            yAxis = d3.svg.axis().scale(y).ticks(5).orient('right');

        var area = d3.svg.area()
            .interpolate('monotone')
            .x(function (d) { return x(d.x); })
            .y0(height)
            .y1(function (d) { return y(d.y); });

        var line = d3.svg.line()
            .interpolate('monotone')
            .x(function (d) { return x(d.x); })
            .y(function (d) { return y(d.y); });

        var tip = d3.tip()
            .attr('class', 'd3-tip')
            .offset([-10, 0])
            .html(function (d) {
            return '<strong>' + tooltipX + ' </strong> <span style=color:red>' + formatTime(new Date(d.x)) + '</span><br><strong>' + tooltipY + ' </strong> <span style=color:red>' + d.y + '</span>';
        });

        function dataInit () {
            data.forEach(type);
            data.sort(function (a, b) { return d3.ascending(a.x, b.x); });
        };

        function domainInit() {
            x.domain([data[0].x, data[data.length - 1].x]);
            y.domain([0, d3.max(data, function (d) { return d.y; })]).nice();
        };


        function toolTipInit (d) {
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
           .on('mouseover', tip.show)
           .on('mouseout', tip.hide);
        };

        function svgInit() {
            svg = d3.select(container).append('svg')
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
                .text(graphLabel);


            toolTipInit(data);
            svg.call(tip);

        };

        function click() {
            var n = data.length - 1,
            i = Math.floor(Math.random() * n / 2),
            j = i + Math.floor(Math.random() * n / 2) + 1;

            x.domain([data[i].x, data[j].x]);
            var newData = data.slice(i, j + 1);
            var t = svg.transition().duration(500);
            t.select('.x.axis').call(xAxis);
            t.select('.area').attr('d', area(data));
            t.select('.line').attr('d', line(data));

            toolTipInit(newData);
        };

        function type(d) {
            d.x = parse(d.x);
            d.y = +d.y;
            return d;
        };

        (function() {
            dataInit();
            domainInit();
            svgInit();
        })();
    };

})(jQuery, d3);
