using System.ComponentModel.DataAnnotations;
using Mars.Generators;
using Mars.Generators.ApplicationGenerators;
using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Generators;

namespace Mars.Api;

[GenerateCrud]
public class Currency
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string Symbol { get; set; } = "";
    public int? MyNumber { get; set; }
    public DateTime Dt1 { get; set; }
    public DateOnly Dt2 { get; set; }
    public DateTimeOffset Dt3 { get; set; }
}