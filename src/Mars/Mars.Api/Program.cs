using System.Reflection;
using ITech.Cqrs.Cqrs;
using ITech.Cqrs.Cqrs.ApplicationEvents;
using Mars.Api;
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

app.MapGet("/weatherforecast", async (string name, MarsDb db) =>
{
    var result = await db.Set<Country>()
        .Where(x => x.Name.ToLower().Contains(name.ToLower()))
        .ToListAsync();
    return TypedResults.Ok(result);
});


// This is required to get access to generated endpoints
app.MapGeneratedEndpoints();

app.Run();