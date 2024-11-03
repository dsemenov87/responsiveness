namespace responsiveness.Components.Models;

public class LinePlotSettings
{
        public required string ElementId { get; init; }
        public required string Title { get; init; }
        public List<LinePlotData> Data { get; } = new List<LinePlotData>();
        public required int Width { get; init; }
        public required int Height { get; init; }
        public string? HtmlClass { get; init; }
}