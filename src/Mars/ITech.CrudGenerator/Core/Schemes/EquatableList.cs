using System;
using System.Collections.Generic;
using System.Linq;

namespace ITech.CrudGenerator.Core.Schemes;

public class EquatableList<T> : List<T>, IEquatable<EquatableList<T>>
{
    public EquatableList()
    {
    }

    public EquatableList(IEnumerable<T> collection) : base(collection)
    {
    }

    public bool Equals(EquatableList<T>? other)
    {
        if (other is null || Count != other.Count)
        {
            return false;
        }

        for (var i = 0; i < Count; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(this[i], other[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as EquatableList<T>);
    }

    public override int GetHashCode()
    {
        return Count == 0 ? 0 : this.Select(item => item?.GetHashCode() ?? 0).Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(EquatableList<T>? list1, EquatableList<T>? list2)
    {
        return ReferenceEquals(list1, list2) || (list1 is not null && list2 is not null && list1.Equals(list2));
    }

    public static bool operator !=(EquatableList<T> list1, EquatableList<T> list2)
    {
        return !(list1 == list2);
    }
}