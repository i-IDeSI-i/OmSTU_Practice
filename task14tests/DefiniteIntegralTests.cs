using Xunit;
using task14;

public class DefiniteIntegralTests
{
    private static readonly Func<double, double> X = x => x;
    private static readonly Func<double, double> Sin = x => Math.Sin(x);
    private static readonly Func<double, double> Square = x => x * x;

    [Fact]
    public void Integral_OfX_OnSymmetricInterval_IsZero()
    {
        var result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
        Assert.Equal(0, result, 1e-4);
    }

    [Fact]
    public void Integral_OfSin_OnSymmetricInterval_IsZero()
    {
        var result = DefiniteIntegral.Solve(-1, 1, Sin, 1e-5, 8);
        Assert.Equal(0, result, 1e-4);
    }

    [Fact]
    public void Integral_OfX_FromZeroToFive_IsTen()
    {
        var result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);
        Assert.Equal(12.5, result, 1e-4);
    }

    [Fact]
    public void Integral_OfSquare_FromZeroToThree_IsNine()
    {
        var result = DefiniteIntegral.Solve(0, 3, Square, 1e-5, 4);
        Assert.Equal(9, result, 1e-3);
    }

    [Fact]
    public void SingleThread_GivesSameResult()
    {
        var multi = DefiniteIntegral.Solve(0, 5, X, 1e-5, 8);
        var single = DefiniteIntegral.Solve(0, 5, X, 1e-5, 1);
        Assert.Equal(single, multi, 1e-4);
    }
}