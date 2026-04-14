using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Xunit;

namespace NCoreUtils.Extensions.Unit;

internal sealed class CaseInsensitiveStringComparer : IEqualityComparer<string>
{
    public static CaseInsensitiveStringComparer Singleton { get; } = new();

    public bool Equals(string? x, string? y)
        => StringComparer.InvariantCultureIgnoreCase.Equals(x, y);

    public int GetHashCode([DisallowNull] string obj)
        => StringComparer.InvariantCultureIgnoreCase.GetHashCode(obj);
}

internal partial class SomeObservable : NotifyPropertyBase
{
    /// <summary>
    /// Some generic string value.
    /// </summary>
    [HasObservableProperty]
    private string? stringValue;

    [HasObservableProperty(Strategy = ChangeTriggerStrategy.InitialSet)]
    private int intValue;

    /// <summary>
    /// Some string value that is comared in the case insensitive manner.
    /// </summary>
    [HasObservableProperty(EqualityComparer = typeof(CaseInsensitiveStringComparer))]
    private string? caseInsensitiveValue;
}

public class ObservableTests
{
    [Fact]
    public void Basic()
    {
        var obj = new SomeObservable();
        var tracker = new Dictionary<string, int>();
        void CheckCount(string propertyName, int expected)
            => Assert.Equal(expected, tracker.TryGetValue(propertyName, out var i) ? i : 0);
        obj.PropertyChanged += (__, e) =>
        {
            ++CollectionsMarshal.GetValueRefOrAddDefault(tracker, e.PropertyName ?? string.Empty, out _);
        };
        obj.StringValue = null;
        CheckCount(nameof(SomeObservable.StringValue), 0);
        obj.StringValue = "xasd";
        CheckCount(nameof(SomeObservable.StringValue), 1);
        obj.IntValue = 0;
        CheckCount(nameof(SomeObservable.IntValue), 1);
        obj.IntValue = 1;
        CheckCount(nameof(SomeObservable.IntValue), 2);
        obj.IntValue = 2;
        CheckCount(nameof(SomeObservable.IntValue), 3);
        obj.CaseInsensitiveValue = "xasd";
        CheckCount(nameof(SomeObservable.CaseInsensitiveValue), 1);
        obj.CaseInsensitiveValue = "XaSd";
        CheckCount(nameof(SomeObservable.CaseInsensitiveValue), 1);
        obj.CaseInsensitiveValue = "XbSd";
        CheckCount(nameof(SomeObservable.CaseInsensitiveValue), 2);
    }
}