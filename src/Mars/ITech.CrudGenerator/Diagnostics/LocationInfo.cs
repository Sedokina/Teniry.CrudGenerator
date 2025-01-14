using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ITech.CrudGenerator.Diagnostics;

internal record LocationInfo(string FilePath, TextSpan TextSpan, LinePositionSpan LineSpan)
{
    public string FilePath { get; } = FilePath;
    public TextSpan TextSpan { get; } = TextSpan;
    public LinePositionSpan LineSpan { get; } = LineSpan;

    public Location ToLocation() => Location.Create(FilePath, TextSpan, LineSpan);

    public static LocationInfo? CreateFrom(SyntaxNode node) => CreateFrom(node.GetLocation());

    public static LocationInfo? CreateFrom(Location? location)
    {
        if (location?.SourceTree is null)
        {
            return null;
        }

        return new LocationInfo(location.SourceTree.FilePath, location.SourceSpan, location.GetLineSpan().Span);
    }
}