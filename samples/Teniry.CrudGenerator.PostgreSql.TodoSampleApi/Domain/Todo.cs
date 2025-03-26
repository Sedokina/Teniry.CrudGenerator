using System.ComponentModel.DataAnnotations;

namespace Teniry.CrudGenerator.PostgreSql.TodoSampleApi.Domain;

public class Todo {
    [Key]
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Description { get; set; } = "";

    public bool Completed { get; set; }
}