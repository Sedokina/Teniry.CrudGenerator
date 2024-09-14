using System;
using System.IO;
using System.Reflection;

namespace Mars.Generators.Extensions;

public static class EmbeddedResourceExtensions
{
    public static string GetEmbeddedResource(string path, Assembly assembly)
    {
        using var stream = assembly.GetManifestResourceStream(path);

        using var streamReader = new StreamReader(stream ?? throw new InvalidOperationException());

        return streamReader.ReadToEnd();
    }
}