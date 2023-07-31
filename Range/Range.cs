namespace Range;

public readonly struct Range
{
    private static readonly FloatComparer Comparator = FloatComparer.Instance;

    public float Start { get; }

    public float End { get; }

    public Range(float start, float end)
    {
        if (Comparator.Compare(start, end) > 0)
        {
            throw new ArgumentException($"end cannot be greater than start");
        }

        Start = start;
        End = end;
    }

    public bool HasIntersection(in Range otherRange)
        =>  IsPointInside(otherRange.End)
            || IsPointInside(otherRange.Start)
            || otherRange.IsPointInside(Start)
            || otherRange.IsPointInside(End);

    private bool IsPointInside(float point)
        => Comparator.Compare(point, Start) >= 0 && Comparator.Compare(point, End) <= 0;


    public Range Merge(in Range otherRange)
    {
        if (Comparator.Compare(End, otherRange.Start) < 0)
        {
            throw new InvalidOperationException("Can't merge intervals");
        }

        var newStart = Comparator.Compare(Start, otherRange.Start) <= 0 ? Start : otherRange.Start;
        var newEnd = Comparator.Compare(End, otherRange.End) >= 0 ? End : otherRange.End;

        return new Range(newStart, newEnd);
    }

    public IEnumerable<Range> Except(Range range)
    {
        if (range.IsFullyIncludes(this))
        {
            yield break;
        }

        if (IsFullyIncludes(range))
        {
            yield return new Range(Start, range.Start);
            yield return new Range(range.End, End);
            yield break;
        }

        if (Comparator.Compare(Start, range.Start) <= 0)
        {
            yield return new Range(Start, range.Start);
        }

        else if (Comparator.Compare(End, range.End) >= 0)
        {
            yield return new Range(range.End, End);
        }
    }

    public bool IsFullyIncludes(in Range current)
        => Comparator.Compare(Start, current.Start) <= 0
           && Comparator.Compare(End, current.End) >= 0;

    public static bool operator ==(Range lhs, Range rhs)
        => Comparator.Compare(lhs.Start, rhs.Start) == 0
           && Comparator.Compare(lhs.End, rhs.End) == 0;

    public static bool operator !=(Range lhs, Range rhs)
        => Comparator.Compare(lhs.Start, rhs.Start) != 0
           || Comparator.Compare(lhs.End, rhs.End) != 0;

    public override bool Equals(object? obj)
    {
        if (obj is not Range)
        {
            return false;
        }

        var range = (Range)obj;

        return Comparator.Compare(Start, range.Start) == 0
               && Comparator.Compare(End, range.End) == 0;
    }

    public override int GetHashCode()
        => HashCode.Combine(Start, End);
}

public class FloatComparer : IComparer<float>
{
    private static readonly Lazy<FloatComparer> Lazy =
        new(() => new FloatComparer());

    public static FloatComparer Instance => Lazy.Value;

    public int Compare(float x, float y)
    {
        var diff = Math.Abs(x - y);

        if (diff < float.Epsilon)
        {
            return 0;
        }

        return x > y ? 1 : -1;
    }
}