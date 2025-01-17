namespace ITech.CrudGenerator.TestApi.Generators.SimpleEntityGenerator;

/// <summary>
///     This is a basic entity to test simple case of CQRS generation
/// </summary>
public class SimpleEntity {
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}