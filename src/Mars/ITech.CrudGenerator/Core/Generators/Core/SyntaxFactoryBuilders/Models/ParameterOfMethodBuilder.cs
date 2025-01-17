using Microsoft.CodeAnalysis.CSharp;

namespace ITech.CrudGenerator.Core.Generators.Core.SyntaxFactoryBuilders.Models;

internal class ParameterOfMethodBuilder {
    /// <summary>
    ///     This modifier is requrired, if for example you need SyntaxKind.ThisKeyword for static method
    /// </summary>
    public SyntaxKind[] Modifiers { get; set; }

    public string Type { get; set; }
    public string Name { get; set; }

    public ParameterOfMethodBuilder(SyntaxKind[] modifiers, string type, string name) {
        Modifiers = modifiers;
        Type = type;
        Name = name;
    }

    public ParameterOfMethodBuilder(string type, string name) : this([], type, name) { }
}