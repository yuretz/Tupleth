using System.Runtime.CompilerServices;
using Xunit.Sdk;

namespace Tupleth.Tests;


public class LispTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    [InlineData(42)]
    [InlineData("foo")]
    public void QuoteAtom(object? value) =>
        AssertResult(value, Run((quote, value)));

    [Fact]
    public void QuoteTuple1()
    {
        var value = default(ValueTuple);
        AssertResult(value, Run((quote, value)));
    }

    [Fact]
    public void QuoteTuple2()
    {
        var value = (1, 2, 3);
        AssertResult(value, Run((quote, value)));
    }

    [Fact]
    public void QuoteTuple3()
    {
        var value = ((1, "foo"), 3, (nil, 4, 5, ValueTuple.Create(true)));
        AssertResult(value, Run((quote, value)));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(42)]
    [InlineData("foo")]
    public void CarAtom(object value)
    {
        Assert.ThrowsAny<Exception>(() => Run((car, value)));
    }

    [Fact]
    public void CarNil1()
    {
        AssertResult(nil, Run((car, nil)));
    }

    [Fact]
    public void CarNil2()
    {
        AssertResult(nil, Run((car, (quote, nil))));
    }

    [Fact]
    public void CarList1()
    {
        var value = (1, 2, 3);
        AssertResult(value.Item1, Run((car, (quote, value))));
    }

    [Fact]
    public void CarList2()
    {
        var value = ("foo", "bar", "baz");
        AssertResult(value.Item1, Run((car, (quote, value))));
    }

    [Fact]
    public void CarList3()
    {
        var value = (("foo", "bar"), "baz");
        AssertResult(value.Item1, Run((car, (quote, value))));
    }

    [Fact]
    public void CarList4()
    {
        var value = Tuple.Create(42);
        AssertResult(value.Item1, Run((car, (quote, value))));
    }

    [Fact]
    public void CdrNil1()
    {
        AssertResult(nil, Run((cdr, nil)));
    }

    [Fact]
    public void CdrNil2()
    {
        AssertResult(nil, Run((cdr, (quote, nil))));
    }
    [Fact]
    public void CdrList1()
    {
        var value = (1, 2, 3);
        AssertResult(value.ToEnumerable().Skip(1), Run((cdr, (quote, value))));
    }

    [Fact]
    public void CdrList2()
    {
        var value = ("foo", "bar", "baz");
        AssertResult(value.ToEnumerable().Skip(1), Run((cdr, (quote, value))));
    }

    [Fact]
    public void CdrList3()
    {
        var value = (("foo", "bar"), "baz");
        AssertResult(value.ToEnumerable().Skip(1), Run((cdr, (quote, value))));
    }

    [Fact]
    public void CdrList4()
    {
        var value = Tuple.Create(42);
        AssertResult(value.ToEnumerable().Skip(1), Run((cdr, (quote, value))));
    }

    private void AssertResult(object? expected, object? actual)
    {
        if(expected is ITuple tuple)
        {
            expected = tuple.ToEnumerable();
        }

        if(expected is IEnumerable<object?> expectedEnumrable
            && actual is IEnumerable<object?> actualEnumerable)
        {
            var expectedList = expectedEnumrable.ToList();
            var actualList = actualEnumerable.ToList();
            Assert.Equal(expectedList.Count, actualList.Count);
            Assert.All(
                Enumerable.Zip(expectedList, actualList), 
                (pair) => AssertResult(pair.First, pair.Second));
        }
        else 
        { 
            Assert.Equal(expected, actual); 
        }
    }

    private static object? Run(ITuple program) =>
        Lisp.Parse(program).Compile()();

}