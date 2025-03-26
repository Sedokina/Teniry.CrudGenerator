using Microsoft.EntityFrameworkCore;
using Teniry.CrudGenerator.Abstractions.DbContext;
using Teniry.CrudGenerator.Mongo.TodoSampleApi.Domain;

namespace Teniry.CrudGenerator.Mongo.TodoSampleApi;

[UseDbContext(DbContextDbProvider.Mongo)]
public class TodoDb(DbContextOptions<TodoDb> options) : DbContext(options) {
    public DbSet<Todo> Todos { get; set; }
}