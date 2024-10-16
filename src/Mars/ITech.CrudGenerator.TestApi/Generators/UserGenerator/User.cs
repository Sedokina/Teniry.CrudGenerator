namespace ITech.CrudGenerator.TestApi.Generators.UserGenerator;

public class User
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
    public Guid NotId { get; set; }
}