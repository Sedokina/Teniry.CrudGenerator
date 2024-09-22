namespace Mars.Generators.ApplicationGenerators.Configurations.Global.Factories;

public class GlobalCrudGeneratorConfigurationDefaultConfigurationFactory
{
    public static GlobalCqrsGeneratorConfigurationBuilder Construct()
    {
        return new GlobalCqrsGeneratorConfigurationBuilder
        {
            AutogeneratedFileText = @"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a crud generator tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------",
            NullableEnable = true,
            TemplatesBasePath = "Mars.Generators.Templates.Crud",
        };
    }
}