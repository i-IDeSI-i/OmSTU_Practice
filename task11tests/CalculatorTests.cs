using Xunit;
using task11;

public class CalculatorTests
{
    [Fact]
    public void Add_ReturnsSum()
    {
        ICalculator calc = CalculatorFactory.Create();
        Assert.Equal(5, calc.Add(2, 3));
    }

    [Fact]
    public void Minus_ReturnsDifference()
    {
        ICalculator calc = CalculatorFactory.Create();
        Assert.Equal(4, calc.Minus(10, 6));
    }

    [Fact]
    public void Mul_ReturnsProduct()
    {
        ICalculator calc = CalculatorFactory.Create();
        Assert.Equal(12, calc.Mul(3, 4));
    }

    [Fact]
    public void Div_ReturnsQuotient()
    {
        ICalculator calc = CalculatorFactory.Create();
        Assert.Equal(5, calc.Div(20, 4));
    }

    [Fact]
    public void Div_ByZero_Throws()
    {
        ICalculator calc = CalculatorFactory.Create();
        Assert.Throws<DivideByZeroException>(() => calc.Div(1, 0));
    }

    [Fact]
    public void CompiledType_ImplementsInterface()
    {
        ICalculator calc = CalculatorFactory.Create();
        Assert.IsAssignableFrom<ICalculator>(calc);
    }
}