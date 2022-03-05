using System;
using System.Collections.Generic;
using System.Linq;
using NCoreUtils.Internal;
using Xunit;

namespace NCoreUtils.Extensions.Unit;

public class StringTests
{
    [Fact]
    public void Supply()
    {
        const string Placeholder = "placeholder";
        const string Value = "xasd";
        Assert.Equal(Placeholder, default(string).Supply(Placeholder));
        Assert.Equal(Placeholder, string.Empty.Supply(Placeholder));
        Assert.Equal(Value, Value.Supply(Placeholder));
    }

    [Theory]
    [InlineData(default(string), StringSplitOptions.None, new string[0])]
    [InlineData("", StringSplitOptions.None, new [] { "" })]
    [InlineData("\r", StringSplitOptions.None, new [] { "" })]
    [InlineData("\n", StringSplitOptions.None, new [] { "", "" })]
    [InlineData("\n\r", StringSplitOptions.None, new [] { "", "" })]
    [InlineData("a\nb", StringSplitOptions.None, new [] { "a", "b" })]
    [InlineData("a\nb\n", StringSplitOptions.None, new [] { "a", "b", "" })]
    [InlineData("a\r\nb", StringSplitOptions.None, new [] { "a", "b" })]
    [InlineData("a\n\rb", StringSplitOptions.None, new [] { "a", "b" })]
    [InlineData("\ra\r\n\rb\r", StringSplitOptions.None, new [] { "a", "b" })]
    [InlineData("\ra\r\n\n\rb\r", StringSplitOptions.None, new [] { "a", "", "b" })]
    [InlineData("\ra\r\n\r\n\rb\r", StringSplitOptions.None, new [] { "a", "", "b" })]
    [InlineData("\r a\r\n\r\n\rb\r", StringSplitOptions.None, new [] { " a", "", "b" })]
    [InlineData("\r a\r\n \r\n\rb\r", StringSplitOptions.None, new [] { " a", " ", "b" })]
    [InlineData(default(string), StringSplitOptions.RemoveEmptyEntries, new string[0])]
    [InlineData("", StringSplitOptions.RemoveEmptyEntries, new string[0])]
    [InlineData("\n", StringSplitOptions.RemoveEmptyEntries, new string[0])]
    [InlineData("a\nb", StringSplitOptions.RemoveEmptyEntries, new [] { "a", "b" })]
    [InlineData("\ra\r\n\n\rb\r", StringSplitOptions.RemoveEmptyEntries, new [] { "a", "b" })]
    [InlineData("\ra\r\n\r\n\rb\r", StringSplitOptions.RemoveEmptyEntries, new [] { "a", "b" })]
    [InlineData("\r a\r\n \r\n\rb\r", StringSplitOptions.RemoveEmptyEntries, new [] { " a", " ", "b" })]
    [InlineData(default(string), StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries, new string[0])]
    [InlineData("", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries, new string[0])]
    [InlineData("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries, new string[0])]
    [InlineData(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries, new string[0])]
    [InlineData(" \n ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries, new string[0])]
    [InlineData("a\nb", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries, new [] { "a", "b" })]
    [InlineData("a   ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries, new [] { "a" })]
    [InlineData("\ra\r\n\n\rb\r", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries, new [] { "a", "b" })]
    [InlineData("\ra\r\n\r\n\rb\r", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries, new [] { "a", "b" })]
    [InlineData("\r a\r\n \r\n\rb\r", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries, new [] { "a", "b" })]
    [InlineData(default(string), StringSplitOptions.TrimEntries, new string[0])]
    [InlineData("", StringSplitOptions.TrimEntries, new [] { "" })]
    [InlineData("\n", StringSplitOptions.TrimEntries, new [] { "", "" })]
    [InlineData(" ", StringSplitOptions.TrimEntries, new [] { "" })]
    [InlineData(" \n ", StringSplitOptions.TrimEntries, new [] { "", "" })]
    [InlineData("\r a\r\n \r\n\rb\r", StringSplitOptions.TrimEntries, new [] { "a", "", "b" })]
    public void SplitIntoLines(string? source, StringSplitOptions options, string[] expected)
    {
        var lines = source.SplitIntoLines(options).ToList();
        Assert.True(lines.SequenceEqual(expected));
    }

    [Fact]
    public void Predicates()
    {
        Assert.True(default(string).IsNullOrEmpty());
        Assert.True(string.Empty.IsNullOrEmpty());
        Assert.False("s".IsNullOrEmpty());
        Assert.False(default(string).IsNotEmpty());
        Assert.False(string.Empty.IsNotEmpty());
        Assert.True("s".IsNotEmpty());
    }

    [Theory]
    [InlineData(default(string), new string[0])]
    [InlineData("", new [] { "" })]
    [InlineData("\r", new [] { "" })]
    [InlineData("\n", new [] { "", "" })]
    [InlineData("\n\r", new [] { "", "" })]
    [InlineData("a\nb", new [] { "a", "b" })]
    [InlineData("a\nb\n", new [] { "a", "b", "" })]
    [InlineData("a\r\nb", new [] { "a", "b" })]
    [InlineData("a\n\rb", new [] { "a", "b" })]
    [InlineData("\ra\r\n\rb\r", new [] { "a", "b" })]
    [InlineData("\ra\r\n\n\rb\r", new [] { "a", "", "b" })]
    [InlineData("\ra\r\n\r\n\rb\r", new [] { "a", "", "b" })]
    [InlineData("\r a\r\n\r\n\rb\r", new [] { " a", "", "b" })]
    [InlineData("\r a\r\n \r\n\rb\r", new [] { " a", " ", "b" })]
    public void SimpleSplitIntoLines(string? source, string[] expected)
    {
        var lines = source.SplitIntoLines().ToList();
        Assert.True(lines.SequenceEqual(expected));
    }

    [Fact]
    public void Exn()
    {
        Assert.Equal(
            "source",
            Assert.Throws<ArgumentNullException>(() => new LineEnumerable(default!)).ParamName
        );
        Assert.Equal(
            "source",
            Assert.Throws<ArgumentNullException>(() => new LineEnumerator(default!)).ParamName
        );
        Assert.Equal(
            "source",
            Assert.Throws<ArgumentNullException>(() => new TrimmedLineEnumerable(default!)).ParamName
        );
        Assert.Equal(
            "source",
            Assert.Throws<ArgumentNullException>(() => new TrimmedLineEnumerator(default!)).ParamName
        );
    }
}