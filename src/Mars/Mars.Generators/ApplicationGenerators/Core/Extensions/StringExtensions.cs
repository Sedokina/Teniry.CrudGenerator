namespace Mars.Generators.ApplicationGenerators.Core.Extensions;

public static class StringExtensions
{
    public static string EnsureEndsWith(
        this string source,
        string suffix)
    {
        if (source.EndsWith(suffix)) return source;
        return source + suffix;
    }

    public static string ToLowerFirstChar(this string str)
    {
        if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
            return str.Length == 1 ? char.ToLower(str[0]).ToString() : char.ToLower(str[0]) + str.Substring(1);

        return str;
    }
}