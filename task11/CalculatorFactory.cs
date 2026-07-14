namespace task11;

public static class CalculatorFactory
{
    public const string CalculatorSource = @"
using task11;

public class Calculator : ICalculator
{
    public int Add(int a, int b) => a + b;
    public int Minus(int a, int b) => a - b;
    public int Mul(int a, int b) => a * b;
    public int Div(int a, int b) => a / b;
}";

    public static ICalculator Create()
        => RuntimeCompiler.CompileAndCreate<ICalculator>(CalculatorSource, "Calculator");
}