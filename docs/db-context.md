# DbContext

Before generating CRUD operations for a class, you have to create DbContext. This DbContext would be used to save
changes to the DB and select data from the db in generated code.

> [!NOTE]
> Currently library uses Entity Framework Core to interact with the database.

> [!WARNING]
> Library was tested with PostgreSql and MongoDb providers for Entity Framework Core. Other providers may be limited in
> functionality or may lead to errors or unexpected behavior.

Use `Npgsql.EntityFrameworkCore.PostgreSQL` for PostgreSql and `MongoDB.EntityFrameworkCore` for MongoDb.

# UseDbContext attribute

`UseDbContext` attribute is used to expose the DbContext to the CRUD generator. When the generator is run, it will look
for DbContext with this attribute and generate CRUD operations using this context.

It has one parameter`DbContextDbProvider` which is used to specify the provider of the DbContext.
`DbContextDbProvider` is an enum with values `Postgres` and `Mongo`.

> [!WARNING]
> Currently generator can work with only one DbContext.

## Error CDG003 : There is no class with UseDbContextAttribute attribute in the project

If you wouldn't specify `UseDbContext` attribute on any DbContext class in the project, you will get a build error
`CDG003 : There is no class with UseDbContextAttribute attribute in the project`.

# Create DbContext

```csharp
using Microsoft.EntityFrameworkCore;
using Teniry.CrudGenerator.Abstractions.DbContext;

[UseDbContext(DbContextDbProvider.Mongo)]
public class TodoDb : DbContext {
    public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }

    public DbSet<Todo> Todos { get; set; }
}
```