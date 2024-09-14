using System;
using System.IO;
using System.Reflection;
using Scriban;

namespace Mars.Generators.ApplicationGenerators.Core;

public abstract class CrudGenerator
{
    internal Template ReadTemplate(string templatePath)
    {
        return Template.Parse(GetEmbeddedResource(templatePath, GetType().Assembly));
    }

    private static string GetEmbeddedResource(string path, Assembly assembly)
    {
        using var stream = assembly.GetManifestResourceStream(path);
        using var streamReader = new StreamReader(stream ?? throw new InvalidOperationException());
        return streamReader.ReadToEnd();
    }
}