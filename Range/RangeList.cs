using System.Collections;

namespace Range;

public class RangeList : ICollection<Range>
{
    private readonly List<Range> _list;

    private static readonly RangeComparer StartRangeComparer = new(range => range.Start);

    public int Count => _list.Count;

    public bool IsReadOnly => false;

    public RangeList()
    {
        _list = new List<Range>();
    }

    public RangeList(int capacity)
    {
        _list = new List<Range>(capacity);
    }

    public RangeList(IEnumerable<Range> collection)
    {
        _list = new List<Range>(collection);
        _list.Sort(StartRangeComparer);

        CheckForNonIntersections();
    }

    private void CheckForNonIntersections()
    {
        if (!_list.Any())
        {
            return;
        }

        var previous = _list.First();
      
        foreach (var current in _list.Skip(1))
        {
            if (current.HasIntersection(previous))
            {
                throw new InvalidOperationException("List contains intersected values");
            }

            previous = current;
        }
    }

    public void Add(Range range)
    {
        var startIntersectingIndex = GetIndexOfFirstIntersectedRange(in range);

        ReBuildForAdding(in range, startIntersectingIndex);
    }

    private int GetIndexOfFirstIntersectedRange(in Range range)
    {
        int possibleStartIntersectingIndex = _list.BinarySearch(range, StartRangeComparer);
        if (possibleStartIntersectingIndex < 0)
        {
            possibleStartIntersectingIndex = ~possibleStartIntersectingIndex;
        }
        var startIntersectingIndex = possibleStartIntersectingIndex;

        if (HasIntersectionWithRangeToTheLeftOfIndex(in range, possibleStartIntersectingIndex))
        {
            startIntersectingIndex--;
        }

        return startIntersectingIndex;
    }

    private bool HasIntersectionWithRangeToTheLeftOfIndex(in Range range, int index)
    {
        if (index == 0)
        {
            return false;
        }

        var left = _list.ElementAtOrDefault(index - 1);

        if (left == default)
        {
            return false;
        }

        return left.HasIntersection(range);
    }

    private void ReBuildForAdding(in Range range, int nextIndex)
    {
        var next = _list.ElementAtOrDefault(nextIndex);

        if (next == default)
        {
            _list.Insert(nextIndex, range);
            return;
        }

        if (!range.HasIntersection(next))
        {
            _list.Insert(nextIndex, range);
            return;
        }

        var current = range;
        var insertIndex = nextIndex;
        var deleteIndices = new HashSet<int>();

        while (current.HasIntersection(next) && next != default)
        {
            deleteIndices.Add(nextIndex);
            var @new = current.Merge(next);
            current = @new;
            next = _list.ElementAtOrDefault(++nextIndex);
        }

        foreach (var deleteIndex in deleteIndices.Reverse())
        {
            _list.RemoveAt(deleteIndex);
        }

        _list.Insert(insertIndex, current);
    }

    public bool Remove(Range range)
    {
        var startIntersectingIndex = GetIndexOfFirstIntersectedRange(in range);

        return ReBuildForRemoving(in range, startIntersectingIndex);
    }

    private bool ReBuildForRemoving(in Range range, int currentIndex)
    {
        var current = _list.ElementAtOrDefault(currentIndex);
        if (current == default)
        {
            return false;
        }

        var deleteIndices = new HashSet<int>();
        var toAdd = new List<Range>();
        var insertIndex = currentIndex;

        while (current.HasIntersection(range) && current != default)
        {
            deleteIndices.Add(currentIndex);
            toAdd.AddRange(current.Except(range));
            current = _list.ElementAtOrDefault(++currentIndex);
        }

        foreach (var deleteIndex in deleteIndices.Reverse())
        {
            _list.RemoveAt(deleteIndex);
        }

        for (int i = 0; i < toAdd.Count; i++)
        {
            _list.Insert(i + insertIndex, toAdd[i]);
        }

        return deleteIndices.Any();
    }

    public void Clear()
        => _list.Clear();

    public IEnumerator<Range> GetEnumerator()
        => _list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _list.GetEnumerator();

    public bool Contains(Range item)
        => _list.Contains(item);

    public void CopyTo(Range[] array, int arrayIndex)
        => _list.CopyTo(array, arrayIndex);
}