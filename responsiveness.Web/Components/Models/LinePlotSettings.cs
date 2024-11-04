namespace responsiveness.Components.Models;

public class LinePlotSettings
{
        public required string ElementId { get; init; }
        public required string Title { get; init; }
        public List<List<LinePlotData>> Data { get; } = [];
        public required int Width { get; init; }
        public required int Height { get; init; }
}