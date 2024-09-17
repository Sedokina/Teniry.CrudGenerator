using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Core.Extensions;

public static class TypeExtensions
{
    /// <summary>
    ///     Returns true if type is primitive, nullable primitive or has type converter from and to primitive type
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <returns>
    ///     This returns true for:<br />
    ///     - All primitive types<br />
    ///     - All enums<br />
    ///     - strings<br />
    ///     - decimals<br />
    ///     - DateTime<br />
    ///     - DateTimeOffset<br />
    ///     - TimeSpan<br />
    ///     - Uri<br />
    ///     - Guid<br />
    ///     - Nullable of any of the types above<br />
    ///     - numerous other types that have native TypeConverters implemented (see here on the Derived section)<br />
    ///     This approach works well since most frameworks support TypeConverters natively,
    ///     like XML and Json serialization libraries, and you can then use the same converter to parse the values while
    ///     reading.
    /// </returns>
    /// <remarks>
    ///     From: https://stackoverflow.com/a/65079923/10837606
    /// </remarks>
    public static bool IsSimple(this ITypeSymbol type)
    {
        switch (type.SpecialType)
        {
            case SpecialType.System_Boolean:
            case SpecialType.System_SByte:
            case SpecialType.System_Int16:
            case SpecialType.System_Int32:
            case SpecialType.System_Int64:
            case SpecialType.System_Byte:
            case SpecialType.System_UInt16:
            case SpecialType.System_UInt32:
            case SpecialType.System_UInt64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Char:
            case SpecialType.System_String:
            case SpecialType.System_DateTime:
            case SpecialType.System_Decimal:
                return true;
            default:
                if (type.NullableAnnotation == NullableAnnotation.Annotated)
                    return IsSimple(((INamedTypeSymbol)type).TypeArguments[0]);

                if (type.IsValueType && type.IsSealed && type.IsUnmanagedType) return true;

                return false;
        }
    }

    public static bool IsRangeType(this ITypeSymbol type)
    {
        if (type.SpecialType == SpecialType.System_Boolean ||
            type.SpecialType == SpecialType.System_Char ||
            type.SpecialType == SpecialType.System_String)
        {
            return false;
        }

        return IsSimple(type);
    }
}