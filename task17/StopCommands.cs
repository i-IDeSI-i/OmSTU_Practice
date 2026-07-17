namespace task17;

public class HardStopCommand : ICommand
{
    private readonly ServerThread _server;

    public HardStopCommand(ServerThread server) => _server = server;

    public void Execute()
    {
        if (Environment.CurrentManagedThreadId != _server.ManagedThreadId)
            throw new InvalidOperationException(
                "HardStop может быть выполнена только в целевом потоке.");

        _server.SignalHardStop();
    }
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _server;

    public SoftStopCommand(ServerThread server) => _server = server;

    public void Execute()
    {
        if (Environment.CurrentManagedThreadId != _server.ManagedThreadId)
            throw new InvalidOperationException(
                "SoftStop может быть выполнена только в целевом потоке.");

        _server.SignalSoftStop();
    }
}