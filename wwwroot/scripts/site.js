SiteFunctions = {
    GenerateLinePlot: function (settings) {
        // set the dimensions and margins of the graph
        const margin = {top: 10, right: 30, bottom: 30, left: 60},
            width = settings.width - margin.left - margin.right,
            height = settings.height - margin.top - margin.bottom;

        // append the svg object to the body of the page
        const svg = d3.select("#" + settings.elementId)
            .append("svg")
                .attr("width", width + margin.left + margin.right)
                .attr("height", height + margin.top + margin.bottom)
            .append("g")
                .attr("transform", `translate(${margin.left},${margin.top})`);

        const data = settings.data;
        
        // group the data: I want to draw one line per group
        const sumstat = d3.group(data, d => d.metricName); // nest function allows to

        // Add X axis --> it is a date format
        const x = d3.scaleLinear()
            .domain(d3.extent(data, function(d) { return d.timestamp; }))
            .range([ 0, width ]);
        svg.append("g")
            .attr("transform", `translate(0, ${height})`)
            .call(d3.axisBottom(x).ticks(5));

        // Add Y axis
        const y = d3.scaleLinear()
            .domain([0, d3.max(data, function(d) { return +d.n; })])
            .range([ height, 0 ]);
        svg.append("g")
            .call(d3.axisLeft(y));

        // color palette
        const color = d3.scaleOrdinal()
            .range(['#e41a1c','#377eb8','#4daf4a','#984ea3','#ff7f00','#ffff33','#a65628','#f781bf','#999999'])

        // Draw the line
        svg.selectAll(".line")
            .data(sumstat)
            .join("path")
            .attr("fill", "none")
            .attr("stroke", function(d){ return color(d[0]) })
            .attr("stroke-width", 1.5)
            .attr("d", function(d){
                return d3.line()
                    .x(function(d) { return x(d.timestamp); })
                    .y(function(d) { return y(+d.n); })
                    (d[1])
            })
    },

    GeneratePie: function (settings) {
        // set the dimensions and margins of the graph
        const width = settings.width,
            height = settings,
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
        const data = {a: 9, b: 20, c:30, d:8, e:12}

// set the color scale
        const color = d3.scaleOrdinal()
            .range(["#98abc5", "#8a89a6", "#7b6888", "#6b486b", "#a05d56"])

// Compute the position of each group on the pie:
        const pie = d3.pie()
            .value(function(d) {return d[1]})
        const data_ready = pie(Object.entries(data))

// Build the pie chart: Basically, each part of the pie is a path that we build using the arc function.
        svg
            .selectAll('whatever')
            .data(data_ready)
            .join('path')
            .attr('d', d3.arc()
                .innerRadius(0)
                .outerRadius(radius)
            )
            .attr('fill', function(d){ return(color(d.data[1])) })
            .attr("stroke", "black")
            .style("stroke-width", "2px")
            .style("opacity", 0.7)
    }
}