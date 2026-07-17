using ScottPlot;
using task17;
using task18;
using task19;

Console.WriteLine("=== Демонстрация длительных операций (task19) ===\n");

var xs = new List<double>();
var ys = new List<double>();
int globalStep = 0;
var dataLock = new object();

void Record(int id)
{
    lock (dataLock)
    {
        xs.Add(id);
        ys.Add(globalStep);
        globalStep++;
    }
}

var server = new SchedulerThread();
server.Start();

for (int id = 0; id < 5; id++)
{
    int localId = id;
    server.EnqueueCommand(new TestCommand(localId, maxCalls: 4, onExecute: () => Record(localId)));
}

Thread.Sleep(500);

Console.WriteLine("\n=== Все команды выполнены. Останавливаем поток ===");
server.Stop();
server.EnqueueCommand(new SimpleStopSignal());
server.Join();
Console.WriteLine($"Поток остановлен. Живой: {server.IsAlive}");

var plt = new Plot();
plt.Add.ScatterPoints(xs.ToArray(), ys.ToArray());
plt.XLabel("Ид задачи");
plt.YLabel("Сквозной шаг");
plt.Title("Сквозной шаг от ид задачи");
plt.SavePng("task19.png", 700, 500);
Console.WriteLine("График сохранён: task19.png");

public class SimpleStopSignal : ICommand
{
    public void Execute() { }
}
