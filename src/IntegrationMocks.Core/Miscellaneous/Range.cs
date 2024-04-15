using System;

namespace IntegrationMocks.Core.Miscellaneous;

public readonly struct Range<T> where T : IComparable<T>, IEquatable<T>
{
    public Range(T min, T max)
    {
        if (min.CompareTo(max) > 0)
        {
            throw new ArgumentException("Min cannot be greater than max.", nameof(min));
        }

        Min = min;
        Max = max;
    }

    public Range(T minAndMax) : this(minAndMax, minAndMax)
    {
    }

    public T Min { get; }

    public T Max { get; }

    public override string ToString()
    {
        return $"[{Min}; {Max}]";
    }

    public bool Contains(T value)
    {
        return Min.CompareTo(value) <= 0 && Max.CompareTo(value) >= 0;
    }
}
