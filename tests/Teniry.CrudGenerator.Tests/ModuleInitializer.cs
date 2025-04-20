using System.Runtime.CompilerServices;

namespace Teniry.CrudGenerator.Tests;

public static class ModuleInitializer {
    [ModuleInitializer]
    public static void Init() {
        VerifySourceGenerators.Initialize();
    }
}