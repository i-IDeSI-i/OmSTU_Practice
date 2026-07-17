using System.Diagnostics;
using ScottPlot;
using task14;

Func<double, double> sin = Math.Sin;
const double A = -100, B = 100;
const double TargetAccuracy = 1e-4;

double exact = Math.Cos(A) - Math.Cos(B);
Console.WriteLine($"Точное значение интеграла: {exact}\n");

// ---------- Подбор шага ----------
double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
double chosenStep = steps[^1];

Console.WriteLine("Подбор шага:");
foreach (var step in steps)
{
    double value = SingleThreadIntegral.Solve(A, B, sin, step);
    double error = Math.Abs(value - exact);
    bool ok = error <= TargetAccuracy;
    Console.WriteLine($"  шаг {step:E0}: ошибка={error:E3} {(ok ? "OK" : "")}");
    if (ok) { chosenStep = step; break; }
}
Console.WriteLine($"Выбран шаг: {chosenStep:E0}\n");

int maxThreads = Environment.ProcessorCount;
const int repeats = 20;

double Measure(Func<double> action)
{
    action();
    double total = 0;
    for (int r = 0; r < repeats; r++)
    {
        var sw = Stopwatch.StartNew();
        action();
        sw.Stop();
        total += sw.Elapsed.TotalMilliseconds;
    }
    return total / repeats;
}

var threadCounts = new List<int>();
var threadTimes = new List<double>();
var parallelTimes = new List<double>();

Console.WriteLine("Замеры по числу потоков (мс):");
Console.WriteLine($"{"потоки",-8}{"Thread",-12}{"Parallel",-12}");
for (int threads = 1; threads <= maxThreads; threads++)
{
    double tThread = Measure(() => DefiniteIntegral.Solve(A, B, sin, chosenStep, threads));
    double tParallel = Measure(() => ParallelIntegral.Solve(A, B, sin, chosenStep, threads));

    threadCounts.Add(threads);
    threadTimes.Add(tThread);
    parallelTimes.Add(tParallel);
    Console.WriteLine($"{threads,-8}{tThread,-12:F3}{tParallel,-12:F3}");
}

double singleTime = Measure(() => SingleThreadIntegral.Solve(A, B, sin, chosenStep));

double bestThreadTime = threadTimes.Min();
int bestThreadN = threadCounts[threadTimes.IndexOf(bestThreadTime)];
double bestParallelTime = parallelTimes.Min();
int bestParallelN = threadCounts[parallelTimes.IndexOf(bestParallelTime)];

double speedupThread = (singleTime - bestThreadTime) / singleTime * 100.0;
double speedupParallel = (singleTime - bestParallelTime) / singleTime * 100.0;

Console.WriteLine($"\nОднопоток (без потоков):           {singleTime:F3} мс");
Console.WriteLine($"Лучший Thread ({bestThreadN} пот.):        {bestThreadTime:F3} мс  ({speedupThread:F1}%)");
Console.WriteLine($"Лучший Parallel ({bestParallelN} пот.):      {bestParallelTime:F3} мс  ({speedupParallel:F1}%)");

var plt = new Plot();
var xs = threadCounts.Select(t => (double)t).ToArray();
var s1 = plt.Add.ScatterLine(xs, threadTimes.ToArray());
s1.LegendText = "Thread (исходная)";
var s2 = plt.Add.ScatterLine(xs, parallelTimes.ToArray());
s2.LegendText = "Parallel (оптимизир.)";
plt.XLabel("Количество потоков");
plt.YLabel("Время вычисления Solve, мс");
plt.Title($"Интеграл sin(x) на [-100,100], шаг {chosenStep:E0}");
plt.ShowLegend();
plt.SavePng("threads_vs_time.png", 700, 500);
Console.WriteLine("\nГрафик сохранён: threads_vs_time.png");

var report =
$@"ОТЧЁТ: оптимальные параметры многопоточного вычисления интеграла
================================================================

Задача: определённый интеграл sin(x) на отрезке [-100, 100]
Требуемая точность: {TargetAccuracy:E0}
Точное значение интеграла: {exact} (cos(-100) - cos(100), косинус чётный)
Число логических ядер процессора: {maxThreads}
Замеры усреднены по {repeats} прогонам.

1. Выбранный размер шага разбиения: {chosenStep:E0}
   — самый крупный шаг из ряда 1e-1..1e-6, при котором ошибка не превышает
     требуемую точность {TargetAccuracy:E0}.

2. Однопоточная версия (без потоков вообще): {singleTime:F3} мс

3. Исходная многопоточная версия (создание потоков через new Thread):
   — лучшее время: {bestThreadTime:F3} мс при {bestThreadN} потоках
   — разница с однопотоком: {speedupThread:F1}%

4. Оптимизированная многопоточная версия (пул потоков через Parallel.For):
   — лучшее время: {bestParallelTime:F3} мс при {bestParallelN} потоках
   — разница с однопотоком: {speedupParallel:F1}%

5. Суть оптимизации: замена ручного создания потоков (new Thread) на Parallel.For,
   работающий на пуле потоков. Это устраняет затраты на создание и уничтожение
   потоков при каждом вызове, а раздельная запись результатов по участкам избавляет
   от синхронизации при суммировании.
";

File.WriteAllText("optimal_parameters.txt", report);
Console.WriteLine("Отчёт сохранён: optimal_parameters.txt");
