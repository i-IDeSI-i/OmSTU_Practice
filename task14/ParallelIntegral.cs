namespace task14;

public class ParallelIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (threadsNumber < 1)
            throw new ArgumentException("Число потоков должно быть не меньше 1.");

        double totalLength = b - a;
        double chunkLength = totalLength / threadsNumber;
        double[] partials = new double[threadsNumber];

        Parallel.For(0, threadsNumber,
            new ParallelOptions { MaxDegreeOfParallelism = threadsNumber },
            i =>
            {
                double localStart = a + i * chunkLength;
                double localEnd = (i == threadsNumber - 1) ? b : localStart + chunkLength;
                partials[i] = IntegrateSegment(localStart, localEnd, function, step);
            });

        return partials.Sum();
    }

    private static double IntegrateSegment(double start, double end, Func<double, double> function, double step)
    {
        if (end <= start)
            return 0.0;

        double sum = 0.0;
        double x = start;

        while (x < end)
        {
            double next = Math.Min(x + step, end);
            double h = next - x;
            sum += h * (function(x) + function(next)) / 2.0;
            x = next;
        }

        return sum;
    }
}