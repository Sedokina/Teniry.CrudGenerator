using Teniry.CrudGenerator.Abstractions.Configuration;

namespace Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomOperationNameEntityGenerator;

public class CustomOperationNameEntityGeneratorConfiguration : EntityGeneratorConfiguration<CustomOperationNameEntity> {
    public CustomOperationNameEntityGeneratorConfiguration() {
        GetByIdOperation = new() {
            Operation = "CustomOpGetById"
        };
        GetListOperation = new() {
            Operation = "CustomOpGetList"
        };
        CreateOperation = new() {
            Operation = "CustomOpCreate"
        };
        DeleteOperation = new() {
            Operation = "CustomOpDelete"
        };
        UpdateOperation = new() {
            Operation = "CustomOpUpdate"
        };
    }
}