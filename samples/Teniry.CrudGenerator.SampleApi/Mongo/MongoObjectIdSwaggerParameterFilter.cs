using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Teniry.CrudGenerator.SampleApi.Mongo;

// This is required for swagger shows ObjectId as string in endpoints
public class MongoObjectIdSwaggerParameterFilter : ISchemaFilter {
    public void Apply(OpenApiSchema schema, SchemaFilterContext context) {
        if (context.Type == typeof(ObjectId)) {
            schema.Type = "string";
            schema.Properties = new Dictionary<string, OpenApiSchema>();
        }
    }
}