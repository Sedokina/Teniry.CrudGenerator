namespace ITech.CrudGenerator.Tests.EndpointsTests.CustomGottenEntityEndpointTests;

public class DeleteCustomGottenEntityEndpointTests
{
    [Theory]
    [InlineData("DeleteCustomGottenEntityEndpoint")]
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