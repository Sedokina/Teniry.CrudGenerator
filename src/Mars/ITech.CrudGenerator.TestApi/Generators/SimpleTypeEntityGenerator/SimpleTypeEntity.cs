namespace ITech.CrudGenerator.TestApi.Generators.SimpleTypeEntityGenerator;

/// <summary>
///     This entity contains all supported simple types
/// </summary>
public class SimpleTypeEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public char Code { get; set; }
    public bool IsActive { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTimeOffset LastSignInDate { get; set; }
    public byte ByteRating { get; set; }
    public short ShortRating { get; set; }
    public int IntRating { get; set; }
    public long LongRating { get; set; }

    public sbyte SByteRating { get; set; }
    public ushort UShortRating { get; set; }
    public uint UIntRating { get; set; }
    public ulong ULongRating { get; set; }

    public float FloatRating { get; set; }
    public double DoubleRating { get; set; }
    public decimal DecimalRating { get; set; }

    /// <summary>
    ///     This field is to test that not any guid works as primary key
    /// </summary>
    public Guid NotIdGuid { get; set; }
}