using System.Reflection;
using ITech.Cqrs.Cqrs;
using ITech.Cqrs.Cqrs.ApplicationEvents;
using ITech.Cqrs.Cqrs.Queries;
using Mars.Api;
using Mars.Api.Application.CountryFeature.GetListCountry;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException(
                           "Connection string with name 'DefaultConnection' for MongoDb not found"
                       );

builder.Services.AddDbContext<MarsDb>(options => options.UseMongoDB(connectionString, "MarsDb"));


// This is required for endpoints to serialize ObjectId as string and deserialize string as ObjectId
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new MongoObjectIdJsonConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // This is required for swagger shows ObjectId as string in endpoints
    options.SchemaFilter<MongoObjectIdSwaggerParameterFilter>();
    
    // Add swagger documentation from an assembly xml file
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// This is required because generated code uses cqrs
builder.Services.AddCqrs();
builder.Services.AddApplicationEvents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", async ([AsParameters] GetCountriesQuery query, IQueryDispatcher queryDispatcher, CancellationToken cancellation) =>
{
    var result = await queryDispatcher.DispatchAsync<GetCountriesQuery, CountriesDto>(query, cancellation);
    return TypedResults.Ok(result);
});


// This is required to get access to generated endpoints
app.MapGeneratedEndpoints();

app.Run();