using Xunit;
using System;
using SpaceBattle.Lib;

namespace SpaceBattle.Tests;
public class VectorTests
{
    
    [Fact]
    public void Vector_SupportsAnyDimension()
    {
        var v = new Vector(1, 2, 3, 4, 5);
        Assert.Equal(5, v.Dimension);
    }


    [Fact]
    public void Addition_ValidVectors_ReturnsCorrectSum()
    {
        var v1 = new Vector(1, -1, 2);
        var v2 = new Vector(-1, 1, -2);
        var expected = new Vector(0, 0, 0);
        Assert.Equal(expected, v1 + v2);
    }


    [Fact]
    public void Addition_DifferentDimensions_ThrowsArgumentException_Case1()
    {
        var v1 = new Vector(1, 2, 3);
        var v2 = new Vector(1, 2);
        Assert.Throws<ArgumentException>(() => v1 + v2);
    }


    [Fact]
    public void Addition_DifferentDimensions_ThrowsArgumentException_Case2()
    {
        var v1 = new Vector(1, 2);
        var v2 = new Vector(1, 2, 3);
        Assert.Throws<ArgumentException>(() => v1 + v2);
    }

    
    [Fact]
    public void Equals_SameCoordinatesDifferentObjects_ReturnsTrue()
    {
        var v1 = new Vector(1, 2);
        var v2 = new Vector(1, 2);
        Assert.True(v1.Equals(v2));
    }

    
    [Fact]
    public void OperatorEqual_SameCoordinatesDifferentObjects_ReturnsTrue()
    {
        var v1 = new Vector(1, 2);
        var v2 = new Vector(1, 2);
        Assert.True(v1 == v2);
    }


    [Fact]
    public void Equals_DifferentCoordinates_ReturnsFalse()
    {
        var v1 = new Vector(1, 2);
        var v2 = new Vector(2, 3);
        Assert.False(v1.Equals(v2));
    }

    
    [Fact]
    public void OperatorNotEqual_DifferentCoordinates_ReturnsTrue()
    {
        var v1 = new Vector(1, 2);
        var v2 = new Vector(2, 3);
        Assert.True(v1 != v2);
    }

    
    [Fact]
    public void GetHashCode_SameCoordinates_HasSameHashCode()
    {
        var v1 = new Vector(5, 10);
        var v2 = new Vector(5, 10);
        Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
    }
}   

