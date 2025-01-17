namespace Teniry.CrudGenerator.Abstractions.Configuration;

public abstract class EntityGeneratorConfiguration<TEntity> where TEntity : class {
    public string? Title { get; set; }
    public string? TitlePlural { get; set; }
    public EntityGeneratorDefaultSort<TEntity>? DefaultSort { get; set; }
    public EntityGeneratorCreateOperationConfiguration? CreateOperation { get; set; }
    public EntityGeneratorDeleteOperationConfiguration? DeleteOperation { get; set; }
    public EntityGeneratorUpdateOperationConfiguration? UpdateOperation { get; set; }
    public EntityGeneratorGetByIdOperationConfiguration? GetByIdOperation { get; set; }
    public EntityGeneratorGetListOperationConfiguration? GetListOperation { get; set; }
}