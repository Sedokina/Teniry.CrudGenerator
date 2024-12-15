using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;

internal class InternalEntityGeneratorConfiguration
{
    public string? Title { get; set; }
    public string? TitlePlural { get; set; }
    public EntityDefaultSort? DefaultSort { get; set; }
    public InternalEntityGeneratorCreateOperationConfiguration? CreateOperation { get; set; }
    public InternalEntityGeneratorDeleteOperationConfiguration? DeleteOperation { get; set; }
    public InternalEntityGeneratorUpdateOperationConfiguration? UpdateOperation { get; set; }
    public InternalEntityGeneratorGetByIdOperationConfiguration? GetByIdOperation { get; set; }
    public InternalEntityGeneratorGetListOperationConfiguration? GetListOperation { get; set; }
}