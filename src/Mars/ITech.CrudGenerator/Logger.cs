using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ITech.CrudGenerator;

internal static class Logger
{
    public static List<string> Logs { get; } = new();

    public static void Print(string msg)
    {
        Logs.Add("/*\n" + msg + "\n*/");
    }

    // public static void FlushLogs(GeneratorExecutionContext context)
    // {
    //     context.AddSource("logs.g.cs", SourceText.From(string.Join("\n", Logs), Encoding.UTF8));
    // }
}