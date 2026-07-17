using task17;
using task18;
using task19;

Console.WriteLine("Демонстрация длительных операций (task19) \n");

var server = new SchedulerThread();
server.Start();

for (int id = 1; id <= 5; id++)
{
    server.EnqueueCommand(new TestCommand(id, maxCalls: 3));
}

Thread.Sleep(500);

Console.WriteLine("\nВсе команды выполнены по 3 раза. Останавливаем поток (HardStop)");

server.Stop();
server.EnqueueCommand(new SimpleStopSignal()); 
server.Join();

Console.WriteLine($"Поток остановлен. Живой: {server.IsAlive}");

public class SimpleStopSignal : ICommand
{
    public void Execute() { }
}