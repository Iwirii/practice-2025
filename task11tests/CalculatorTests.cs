using Xunit;
using task11;
namespace task11tests;

public class CalculatorTests
{
    private readonly ICalculator calc = CalculatorGenerator.Create();

    [Fact] public void AddCalculateCorrectly() => Assert.Equal(13, calc.Add(10, 3));
    [Fact] public void MinusCalculateCorrectly() => Assert.Equal(4, calc.Minus(6, 2));
    [Fact] public void MulCalculateCorrectly() => Assert.Equal(21, calc.Mul(7, 3));
    [Fact] public void DivCalculateCorrectly() => Assert.Equal(6, calc.Div(30, 5));
    [Fact]
    public void DivByZeroThrowsException()
    {
        Assert.Throws<DivideByZeroException>(() =>
        {
            calc.Div(1, 0);
        });
    }
}
