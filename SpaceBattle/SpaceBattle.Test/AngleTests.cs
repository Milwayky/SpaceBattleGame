using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class AngleTests
{
    public AngleTests()
    {
        Angle.d = 8;
    }

    [Fact]
    public void Addition_ShouldWrapAround()
    {
        var a = new Angle(5);
        var b = new Angle(7);
        var result = a + b;

        Assert.Equal(4, result.n);
    }

    [Fact]
    public void Equals_ShouldBeTrue_ForEquivalentAngles()
    {
        var a = new Angle(15);
        var b = new Angle(23);

        Assert.True(a.Equals(b));
    }

    [Fact]
    public void EqualityOperator_ShouldBeTrue_ForEquivalentAngles()
    {
        var a = new Angle(15);
        var b = new Angle(23);

        Assert.True(a == b);
    }

    [Fact]
    public void Equals_ShouldBeFalse_ForDifferentAngles()
    {
        var a = new Angle(1);
        var b = new Angle(2);

        Assert.False(a.Equals(b));
    }

    [Fact]
    public void Equals_ShouldBeFalse_ForNullOrDifferentType()
    {
        var a = new Angle(1);
        Assert.False(a.Equals(null));
        Assert.False(a.Equals("not an angle"));
    }

    [Fact]
    public void InequalityOperator_ShouldBeTrue_ForDifferentAngles()
    {
        var a = new Angle(1);
        var b = new Angle(2);

        Assert.True(a != b);
    }

    [Fact]
    public void GetHashCode_ShouldBeConsistent()
    {
        var a = new Angle(5);
        Assert.Equal(new Angle(13).GetHashCode(), a.GetHashCode());
    }

    [Fact]
    public void Math_Cos_ShouldWorkDirectly()
    {
        var a = new Angle(4);
        double res = Math.Cos(a);
        Assert.Equal(-1.0, res, 5);
    }

    [Fact]
    public void EqualityOperators_ShouldHandleNull()
    {
        var a = new Angle(1);
        Assert.False(a == null);
        Assert.False(null == a);
        Assert.True(a != null);
        Assert.True(null != a);
    }

    [Fact]
    public void Constructor_ShouldHandleNegativeAngles()
    {
        var a = new Angle(-1);
        Assert.Equal(7, a.n);
    }
}

