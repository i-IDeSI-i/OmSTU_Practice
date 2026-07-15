namespace task14;

public class DefiniteIntegral
{
    // a, b — границы отрезка интегрирования
    // function — подынтегральная функция
    // step — размер шага разбиения
    // threadsNumber — число потоков
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (threadsNumber < 1)
            throw new ArgumentException("Число потоков должно быть не меньше 1.");

        double totalLength = b - a;
        double chunkLength = totalLength / threadsNumber;

        double sharedResult = 0.0;

        var threads = new Thread[threadsNumber];

        for (int i = 0; i < threadsNumber; i++)
        {
            double localStart = a + i * chunkLength;
            double localEnd = (i == threadsNumber - 1) ? b : localStart + chunkLength;

            threads[i] = new Thread(() =>
            {
                double partial = IntegrateSegment(localStart, localEnd, function, step);

                AddToShared(ref sharedResult, partial);
            });

            threads[i].Start();
        }

        foreach (var thread in threads)
            thread.Join();

        return sharedResult;
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

    private static void AddToShared(ref double target, double value)
    {
        double initial, computed;
        do
        {
            initial = target;
            computed = initial + value;
        }

        while (Interlocked.CompareExchange(ref target, computed, initial) != initial);
    }
}