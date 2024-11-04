SiteFunctions = {
    
    GenerateLinePlot: function (settings) {
        // set the dimensions and margins of the graph
        const margin = {top: 20, right: 20, bottom: 30, left: 40},
            width = settings.width - margin.left - margin.right,
            height = settings.height - margin.top - margin.bottom;

        const data = settings.data;

        // append the svg object to the body of the page
        const svg = d3.select("#" + settings.elementId).append("svg")
            .attr("width", width + margin.left + margin.right)
            .attr("height", height + margin.top + margin.bottom)
            .attr("viewBox", [0, 0, width + 50, height])
            // .append("g").attr("transform", `translate(${margin.left},${margin.top})`);
        const yTitle = g =>
            g.append("text")
                .attr("font-family", "sans-serif")
                .attr("font-size", 10)
                .attr("y", 10)
                .html(settings.title);

        const yLabel = svg.append("g").call(yTitle);
        const xScale = d3.scaleTime(
            // domain
            [data[0][0].x, data[0][data[0].length - 1].x],
            // range
            [margin.left, width - margin.right]
        );
        // flatten the data into a single array
        const yScalePrices = data.flat().map(d => d.y);
        // and find the max value from that array
        const yMax = d3.max([...yScalePrices, 8]);
        const yScale = d3.scaleLinear([1, yMax], [height - margin.bottom, margin.top]);
        const colors = d3.scaleOrdinal(d3.schemeCategory10);
        const xAxis = d3.axisBottom(xScale);
        const yAxis = d3.axisLeft(yScale);
        const line = d3
            .line()
            .x(d => xScale(d.x))
            .y(d => yScale(d.y))
            .curve(d3.curveNatural);
        const xGrid = g =>
            g.selectAll('line')
                .data(xScale.ticks())
                .join('line')
                .attr('x1', d => xScale(d))
                .attr('x2', d => xScale(d))
                .attr('y1', margin.top)
                .attr('y2', height - margin.bottom)
                .style("stroke-width", 0.2)
                .style("stroke", "black");

        const yGrid = g =>
            g.attr('class', 'grid-lines')
                .selectAll('line')
                .data(yScale.ticks())
                .join('line')
                .attr('x1', margin.left)
                .attr('x2', width - margin.right)
                .attr('y1', d => yScale(d))
                .attr('y2', d => yScale(d))
                .style("stroke-width", 0.2)
                .style("stroke", "black");

        const xgridlines = svg.append("g").call(xGrid);
        const ygridlines = svg.append("g").call(yGrid);

        svg
            .selectAll('path')
            .data(data)
            .join('path')
            .attr('class', 'stock-lines')
            .attr('d', line)
            .style('stroke', (d, i) => colors(d[i].name))
            .style('stroke-width', 2)
            .style('fill', 'transparent');

        svg
            .append('g')
            .attr('class', 'x-axis')
            .attr('transform', `translate(0,${height - margin.bottom})`)
            .call(xAxis);

        svg
            .append('g')
            .attr('class', 'y-axis')
            .attr('transform', `translate(${margin.left},0)`)
            .call(yAxis)
            .selectAll('.domain')
            .remove();

        svg
            .selectAll('text.label')
            .data(data)
            .join('text')
            .attr('class', 'label')
            .attr('x', width - margin.right + 5)
            // The BABA stock name sits right on top of another; let's move it up 12 pixels.
            .attr(
                'y',
                d => yScale(d[d.length - 1].value) + (d[0].name === 'Mean' ? 12 : 0)
            )
            .attr('dy', '0.35em')
            .style('fill', d => colors(d[0].name))
            .style('font-family', 'sans-serif')
            .style('font-size', 12)
            .text(d => d[0].name); 
    },

    GeneratePie: function (settings) {
        // set the dimensions and margins of the graph
        const width = settings.width,
            height = settings.height,
            margin = 40;

// The radius of the pieplot is half the width or half the height (smallest one). I subtract a bit of margin.
        const radius = Math.min(width, height) / 2 - margin;

// append the svg object to the div called 'my_dataviz'
        const svg = d3.select("#" + settings.elementId)
            .append("svg")
            .attr("width", width)
            .attr("height", height)
            .append("g")
            .attr("transform", `translate(${width/2}, ${height/2})`);

// Create dummy data
//         const data = {a: 9, b: 20, c:30, d:8, e:12}
        const data = settings.data;
// set the color scale
        const color = d3.scaleOrdinal().range(d3.schemeSet1);

// Compute the position of each group on the pie:
        const pie = d3.pie()
            .value(function(d) {return d[1]})
        const data_ready = pie(Object.entries(data))

        // shape helper to build arcs:
        const arcGenerator = d3.arc()
            .innerRadius(0)
            .outerRadius(radius);
        
// Build the pie chart: Basically, each part of the pie is a path that we build using the arc function.
        svg
            .selectAll('mySlices')
            .data(data_ready)
            .join('path')
            .attr('d', arcGenerator)
            .attr('fill', function(d){ return(color(d.data[0])) })
            .attr("stroke", "black")
            .style("stroke-width", "2px")
            .style("opacity", 0.7);

        // Now add the annotation. Use the centroid method to get the best coordinates
        svg
            .selectAll('mySlices')
            .data(data_ready)
            .join('text')
            .text(function(d){ return d.data[0]})
            .attr("transform", function(d) { return `translate(${arcGenerator.centroid(d)})`})
            .style("text-anchor", "middle")
            .style("font-size", 10)
    }
}