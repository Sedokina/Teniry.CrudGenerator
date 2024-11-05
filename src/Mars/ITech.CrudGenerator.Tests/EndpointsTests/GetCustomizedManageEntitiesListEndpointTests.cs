namespace ITech.CrudGenerator.Tests.EndpointsTests;

public class GetCustomizedManageEntitiesListEndpointTests
{
    [Theory]
    [InlineData("GetCustomizedManageEntitiesEndpoint")]
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