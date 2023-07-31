namespace Range.Tests;

public class RangeListTests
{
    [Fact]
    public void Insert_IntoEmptyList_ShouldAddNewRange()
    {
        RangeList rangeList = new();

        Range toAdd = new(0.1f, 0.2f);
        rangeList.Add(toAdd);

        Assert.True(rangeList.Count == 1);
        Assert.Contains(toAdd, rangeList);
        Assert.Equal(toAdd, rangeList.First());
    }

    [Fact]
    public void Insert_IntoFirstPositionNoNeedToMergeRanges_ShouldAddNewRange()
    {
        var originalRangeList = CreateRanges();
        RangeList rangeList = new(originalRangeList);

        Range toAdd = new(-0.15f, -0.11f);
        rangeList.Add(toAdd);

        Assert.True(rangeList.Count == originalRangeList.Count() + 1);
        Assert.Contains(toAdd, rangeList);
        Assert.Equal(toAdd, rangeList.First());
    }

    [Fact]
    public void Insert_IntoFirstPositionReplaceFirstWithNewMergedRange_ShouldReplaceFirst()
    {
        var originalRangeList = CreateRanges();
        RangeList rangeList = new(originalRangeList);
        Range toAdd = new(0.05f, 0.11f);
        rangeList.Add(toAdd);

        var mergedRange = new Range(0.01f, 0.11f);

        Assert.True(rangeList.Count == originalRangeList.Count());
        Assert.Contains(mergedRange, rangeList);
        Assert.Equal(mergedRange, rangeList.First());
    }

    [Fact]
    public void Insert_IntoMiddlePositionNoNeedToMergeRanges_ShouldAddNewRange()
    {
        var originalRangeList = CreateRanges();
        RangeList rangeList = new(originalRangeList);

        Range toAdd = new(0.11f, 0.15f);
        rangeList.Add(toAdd);

        Assert.True(rangeList.Count == originalRangeList.Count() + 1);
        Assert.Contains(toAdd, rangeList);
    }

    [Fact]
    public void Insert_IntoLastPositionNoNeedToMergeRanges_ShouldAddNewRange()
    {
        var originalRangeList = CreateRanges();
        RangeList rangeList = new(originalRangeList);

        Range toAdd = new(2.0f, 2.5f);
        rangeList.Add(toAdd);

        Assert.True(rangeList.Count == originalRangeList.Count() + 1);
        Assert.Contains(toAdd, rangeList);
        Assert.Equal(toAdd, rangeList.Last());
    }

    [Fact]
    public void Insert_IntoMiddlePositionReplacingAllFromInsertPositionTillEnd_ShouldBeTheFirstElementAndCreateNew()
    {
        var originalRangeList = CreateRanges();
        RangeList rangeList = new(originalRangeList);

        Range toAdd = new(0.11f, 6.65f);
        rangeList.Add(toAdd);

        Assert.Contains(toAdd, rangeList);
        Assert.True(rangeList.Count == 2);
    }

    [Fact]
    public void Insert_IntoMiddlePositionReplacingAllFromInsertPositionToLastIntersected_ShouldMergeSecondThirdAndForthElements()
    {
        var originalRangeList = CreateRanges();
        RangeList rangeList = new(originalRangeList);

        Range toAdd = new(0.11f, 0.65f);
        rangeList.Add(toAdd);

        var expected = toAdd.Merge(originalRangeList[1])
            .Merge(originalRangeList[2])
            .Merge(originalRangeList[3]);

        Assert.True(rangeList.Count == 3);
        Assert.Contains(expected, rangeList);
    }

    [Fact]
    public void Insert_IntoFirstPositionReplacingAllFromInsertPositionToLastIntersected_ShouldMergeZeroFirstAndSecondElements()
    {
        var originalRangeList = CreateRanges();
        RangeList rangeList = new(originalRangeList);

        Range toAdd = new(0.0f, 0.5f);
        rangeList.Add(toAdd);

        var expected = toAdd.Merge(originalRangeList[0])
            .Merge(originalRangeList[1])
            .Merge(originalRangeList[2]);

        Assert.True(rangeList.Count == 3);
        Assert.Contains(expected, rangeList);
    }


    [Fact]
    public void Remove_FromStartToLastIntersected_ShouldRemoveZeroFirstAndSecondElements()
    {
        var originalRangeList = CreateRanges();
        RangeList rangeList = new(originalRangeList);

        Range toRemove = new(0.0f, 0.5f);
        rangeList.Remove(toRemove);

        Assert.True(rangeList.Count == 2);
    }

    [Fact]
    public void Remove_PassedNotInteresectedRange_ShouldChangeNothing()
    {
        var originalRangeList = CreateRanges();
        RangeList rangeList = new(originalRangeList);

        Range toRemove = new(-0.5f, -0.4f);
        rangeList.Remove(toRemove);

        Assert.True(rangeList.Count == 5);
    }

    [Fact]
    public void Remove_PassedRangeExactlyTheSameAsExistingOne_ShouldRemoveOneRange()
    {
        var originalRangeList = CreateRanges();
        RangeList rangeList = new(originalRangeList);

        Range toRemove = new(0.4f, 0.5f);
        rangeList.Remove(toRemove);

        Assert.True(rangeList.Count == 4);
    }

    [Fact]
    public void Remove_PassedRangePartiallyСrossingTwoRangesAndFullyContainsOne_ShouldRemoveOneAndDecreaseTwo()
    {
        var originalRangeList = CreateRanges();
        RangeList rangeList = new(originalRangeList);

        Range toRemove = new(0.25f, 0.65f);
        var firstExpectedDecrease = originalRangeList[1].Except(toRemove).First();
        var secondExpectedDecrease = originalRangeList[3].Except(toRemove).First();
        rangeList.Remove(toRemove);

        Assert.True(rangeList.Count == 4);
        Assert.Contains(firstExpectedDecrease, rangeList);
        Assert.Contains(secondExpectedDecrease, rangeList);
    }

    [Fact]
    public void Remove_PassedRangeFullyIncludedInExisitingElement_ShouldSplitIntoTwoDecreasedElements()
    {
        var originalRangeList = CreateRanges();
        RangeList rangeList = new(originalRangeList);

        Range toRemove = new(0.42f, 0.46f);
        var splittedArray = originalRangeList[2].Except(toRemove);
        rangeList.Remove(toRemove);

        Assert.True(rangeList.Count == 6);
        Assert.Contains(splittedArray.ElementAt(0), rangeList);
        Assert.Contains(splittedArray.ElementAt(1), rangeList);
    }

    [Fact]
    public void CheckForNonIntersections()
    {
        var rangeList = CreateIntersectedRanges();

        Assert.Throws<InvalidOperationException>(() => new RangeList(rangeList));
    }

    private static IEnumerable<Range> CreateIntersectedRanges()
    {
        return new List<Range>
        {
            new(0.01f, 0.1f),
            new(0.2f, 0.3f),
            new(0.4f, 0.5f),
            new(0.45f, 0.7f),
            new(0.85f, 1.25f),
        };
    }

    private static List<Range> CreateRanges()
    {
        return new List<Range>
        {
            new(0.01f, 0.1f),
            new(0.2f, 0.3f),
            new(0.4f, 0.5f),
            new(0.6f, 0.7f),
            new(0.85f, 1.25f),
        };
    }
}