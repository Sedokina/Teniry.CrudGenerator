using Microsoft.CodeAnalysis.CSharp;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core.SyntaxFactoryBuilders.Models;

internal class ParameterOfMethodBuilder
{
    public string Type { get; set; }
    public string Name { get; set; }
    public SyntaxKind[] Modifiers { get; set; }

    public ParameterOfMethodBuilder(string type, string name, SyntaxKind[]? modifiers = null)
    {
        Type = type;
        Name = name;
        Modifiers = modifiers ?? [];
    }
}