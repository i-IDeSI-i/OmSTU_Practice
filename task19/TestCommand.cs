using task17;
using task18;

namespace task19;

public class TestCommand : ILongRunningCommand
{
    private readonly int _id;
    private readonly int _maxCalls;
    private readonly Action? _onExecute;
    private int _counter = 0;

    public TestCommand(int id, int maxCalls = 3, Action? onExecute = null)
    {
        _id = id;
        _maxCalls = maxCalls;
        _onExecute = onExecute;
    }

    public bool IsCompleted => _counter >= _maxCalls;
    public int Counter => _counter;

    public void Execute()
    {
        if (_counter >= _maxCalls)
            return;

        _counter++;
        Console.WriteLine($"Поток {_id} вызов {_counter}");
        _onExecute?.Invoke();
    }
}
