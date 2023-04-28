namespace IntegrationMocks.Core.Miscellaneous;

public class Range<T> where T : IComparable<T>, IEquatable<T>
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

    public bool Contains(Range<T> other)
    {
        return Min.CompareTo(other.Min) <= 0 && Max.CompareTo(other.Max) >= 0;
    }

    public bool Intersects(Range<T> other)
    {
        return Min.CompareTo(other.Min) <= 0 && Max.CompareTo(other.Min) >= 0
               || other.Min.CompareTo(Min) <= 0 && other.Max.CompareTo(Min) >= 0;
    }
}
