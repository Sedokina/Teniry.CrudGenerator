using System.ComponentModel.DataAnnotations;

namespace Teniry.CrudGenerator.TodoSampleAPI.Domain;

public class Todo {
    /// <summary>
    ///    Unique identifier for the todo item
    /// </summary>
    /// <remarks>
    /// Type: ObjectId or string with [BsonRepresentation(BsonType.ObjectId)]
    /// </remarks>
    [Key]
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Description { get; set; } = "";

    public bool Completed { get; set; }
}