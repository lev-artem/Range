namespace Range;

public class RangeComparer : IComparer<Range>
{
    private static readonly FloatComparer FloatComparator = FloatComparer.Instance;

    private readonly Func<Range, float> _propertySelector;

    public RangeComparer(Func<Range, float> propertySelector)
    {
        _propertySelector = propertySelector ?? throw new ArgumentNullException(nameof(propertySelector));
    }

    public int Compare(Range x, Range y)
    {
        var lhs = _propertySelector(x);
        var rhs = _propertySelector(y); 
        return FloatComparator.Compare(lhs, rhs);
    }
}