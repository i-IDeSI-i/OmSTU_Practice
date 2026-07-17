namespace task14;

public static class SingleThreadIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step)
    {
        if (b <= a)
            return 0.0;

        double sum = 0.0;
        double x = a;

        while (x < b)
        {
            double next = Math.Min(x + step, b);
            double h = next - x;
            sum += h * (function(x) + function(next)) / 2.0;
            x = next;
        }

        return sum;
    }
}