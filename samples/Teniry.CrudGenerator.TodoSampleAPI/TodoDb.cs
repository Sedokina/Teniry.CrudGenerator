using Microsoft.EntityFrameworkCore;
using Teniry.CrudGenerator.Abstractions.DbContext;
using Teniry.CrudGenerator.TodoSampleAPI.Domain;

namespace Teniry.CrudGenerator.TodoSampleAPI;

[UseDbContext(DbContextDbProvider.Mongo)]
public class TodoDb(DbContextOptions<TodoDb> options) : DbContext(options) {
    public DbSet<Todo> Todos { get; set; }
}