using System.ComponentModel.DataAnnotations;
using Mars.Generators;

namespace Mars.Api;

[GenerateCreateCommand]
public class Currency
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string Symbol { get; set; } = "";
}