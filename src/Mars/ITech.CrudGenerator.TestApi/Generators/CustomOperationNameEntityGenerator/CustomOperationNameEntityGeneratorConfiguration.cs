using ITech.CrudGenerator.Abstractions.Configuration;

namespace ITech.CrudGenerator.TestApi.Generators.CustomOperationNameEntityGenerator;

public class CustomOperationNameEntityGeneratorConfiguration : EntityGeneratorConfiguration<CustomOperationNameEntity> {
    public CustomOperationNameEntityGeneratorConfiguration() {
        GetByIdOperation = new EntityGeneratorGetByIdOperationConfiguration {
            Operation = "CustomOpGetById",
        };
        GetListOperation = new EntityGeneratorGetListOperationConfiguration {
            Operation = "CustomOpGetList",
        };
        CreateOperation = new EntityGeneratorCreateOperationConfiguration {
            Operation = "CustomOpCreate",
        };
        DeleteOperation = new EntityGeneratorDeleteOperationConfiguration() {
            Operation = "CustomOpDelete",
        };
        UpdateOperation = new EntityGeneratorUpdateOperationConfiguration {
            Operation = "CustomOpUpdate",
        };
    }
}