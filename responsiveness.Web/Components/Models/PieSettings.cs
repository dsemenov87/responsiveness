namespace responsiveness.Components.Models;

public class PieSettings
{
    public required string ElementId { get; init; }
    public required string Title { get; init; }
    public Dictionary<string, double> Data { get; } = new();
    public required int Width { get; init; }
    public required int Height { get; init; }
    public string? HtmlClass { get; init; }
}