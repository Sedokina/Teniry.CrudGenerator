namespace ITech.CrudGenerator.Tests.EndpointsTests.CustomGottenEntityEndpointTests;

public class UpdateCustomGottenEntityEndpointTests
{
    [Theory]
    [InlineData("UpdateCustomGottenEntityEndpoint")]
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