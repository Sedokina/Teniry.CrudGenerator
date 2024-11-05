namespace ITech.CrudGenerator.Tests.HandlersTests.CustomizedManageEntityHandlersTests;

public class GetCustomManagedEntityHandlerTests
{
    [Theory]
    [InlineData("GetCustomManagedEntityQuery")]
    [InlineData("GetCustomManagedEntityHandler")]
    public void Should_NotGenerateGetHandler(string typeName)
    {
        // Act
        var foundTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.Name.Equals(typeName));

        // Assert
        foundTypes.Should().BeEmpty();
    }
}