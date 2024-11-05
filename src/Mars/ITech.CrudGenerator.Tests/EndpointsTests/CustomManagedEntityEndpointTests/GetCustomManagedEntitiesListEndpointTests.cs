namespace ITech.CrudGenerator.Tests.EndpointsTests.CustomManagedEntityEndpointTests;

public class GetCustomManagedEntitiesListEndpointTests
{
    [Theory]
    [InlineData("GetCustomManagedEntitiesEndpoint")]
    public void Should_NotGenerateEndpointClass(string typeName)
    {
        // Act
        var foundTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.Name.Equals(typeName));
        
        // Assert
        foundTypes.Should().BeEmpty();
    }
}