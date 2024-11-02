using ITech.CrudGenerator.Abstractions.Configuration;

namespace ITech.CrudGenerator.TestApi.Generators.SimpleTypeDefaultSortEntityGenerator;

public class SimpleTypeDefaultSortEntityGeneratorConfiguration
    : EntityGeneratorConfiguration<SimpleTypeDefaultSortEntity>
{
    public SimpleTypeDefaultSortEntityGeneratorConfiguration()
    {
        DefaultSort = new EntityGeneratorDefaultSort<SimpleTypeDefaultSortEntity>("desc", x => x.Name);
    }
}