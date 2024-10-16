using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CompanyFeature.GetCompanies;
using ITech.CrudGenerator.TestApi.Generators.CompanyGenerator;
using Moq;
using Moq.EntityFrameworkCore;

namespace ITech.CrudGenerator.Tests.HandlersTests;

public class GetListHandlerTests
{
    private readonly Mock<TestMongoDb> _db;
    private readonly GetCompaniesQuery _query;
    private readonly GetCompaniesHandler _sut;

    public GetListHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new GetCompaniesHandler(_db.Object);
        _query = new GetCompaniesQuery
        {
            Page = 1,
            PageSize = 10
        };
    }


    [Fact]
    public async Task Should_ChangeEntityDataAndSave()
    {
        // Arrange
        _db.Setup(x => x.Set<Company>()).ReturnsDbSet([new Company { Id = Guid.NewGuid(), Name = "My company" }]);

        // Act
        var companies = await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        companies.Page.Should().NotBeNull();
        companies.Page.CurrentPageIndex.Should().Be(1);
        companies.Page.PageSize.Should().Be(10);
        companies.Items.Should().SatisfyRespectively(dto =>
        {
            dto.Id.Should().NotBeEmpty();
            dto.Name.Should().NotBeEmpty();
        });
    }
}