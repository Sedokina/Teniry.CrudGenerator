using Microsoft.EntityFrameworkCore;
using Teniry.CrudGenerator.Abstractions.DbContext;
using Teniry.CrudGenerator.PostgreSql.TodoSampleApi.Domain;

namespace Teniry.CrudGenerator.PostgreSql.TodoSampleApi;

[UseDbContext(DbContextDbProvider.Mongo)]
public class TodoDb(DbContextOptions<TodoDb> options) : DbContext(options) {
    public DbSet<Todo> Todos { get; set; }
}