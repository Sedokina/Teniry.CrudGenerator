namespace ITech.CrudGenerator.Tests.HandlersTests.CustomizedManageEntityHandlersTests;

public class GetCustomizedManageEntityHandlerTests
{
    [Theory]
    [InlineData("GetCustomizedManageEntityQuery")]
    [InlineData("GetCustomizedManageEntityHandler")]
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