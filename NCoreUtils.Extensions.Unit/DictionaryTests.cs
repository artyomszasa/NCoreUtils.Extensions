using System;
using System.Collections.Generic;
using Xunit;

namespace NCoreUtils.Extensions.Unit;

public class DictionaryTests
{
#pragma warning disable CS0618
    [Fact]
    public void GetOrDefault()
    {
        Assert.Throws<ArgumentNullException>(() => DictionaryExtensions.GetOrDefault<int, int>(null!, 2));

        var d = new Dictionary<int, int> { { 2, 3 } };

        Assert.Equal(3, d.GetOrDefault(2));
        Assert.Equal(default, d.GetOrDefault(3));
        Assert.Equal(5, d.GetOrDefault(3, 5));
    }
#pragma warning restore CS0618

    [Fact]
    public void GetOrSupply()
    {
        var d = new Dictionary<int, int> { { 2, 3 } };

        Assert.Throws<ArgumentNullException>(() => DictionaryExtensions.GetOrSupply(null!, 2, () => 3));
        Assert.Throws<ArgumentNullException>(() => DictionaryExtensions.GetOrSupply(d, 2, null!));

        // supply not called
        Assert.Equal(3, d.GetOrSupply(2, () => throw new InvalidOperationException("Should not be called")));
        Assert.Equal(5, d.GetOrSupply(3, () => 5));
    }
}