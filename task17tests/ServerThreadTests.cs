using Xunit;
using task17;

public class ServerThreadTests
{
    private class CountingCommand : ICommand
    {
        private readonly Action _action;
        public CountingCommand(Action action) => _action = action;
        public void Execute() => _action();
    }

    [Fact]
    public void ExecutesEnqueuedCommands()
    {
        var server = new ServerThread();
        int counter = 0;
        server.Start();

        for (int i = 0; i < 5; i++)
            server.EnqueueCommand(new CountingCommand(() => Interlocked.Increment(ref counter)));

        server.EnqueueCommand(new SoftStopCommand(server));
        server.Join();

        Assert.Equal(5, counter);
    }

    [Fact]
    public void SoftStop_ExecutesAllQueuedCommandsBeforeStopping()
    {
        var server = new ServerThread();
        int counter = 0;
        server.Start();

        for (int i = 0; i < 3; i++)
            server.EnqueueCommand(new CountingCommand(() => Interlocked.Increment(ref counter)));

        server.EnqueueCommand(new SoftStopCommand(server));
        server.Join();

        Assert.Equal(3, counter);
        Assert.False(server.IsAlive);
    }

[Fact]
    public void HardStop_StopsImmediately_EvenWithQueuedCommands()
    {
        var server = new ServerThread();
        int counter = 0;
        var gate = new ManualResetEventSlim(false);
        server.Start();

        server.EnqueueCommand(new CountingCommand(() => gate.Wait()));

        server.EnqueueCommand(new HardStopCommand(server));

        for (int i = 0; i < 100; i++)
            server.EnqueueCommand(new CountingCommand(() => Interlocked.Increment(ref counter)));

        gate.Set();
        server.Join();

        Assert.Equal(0, counter);
        Assert.False(server.IsAlive);
    }

    [Fact]
    public void StopCommand_FromWrongThread_Throws()
    {
        var server = new ServerThread();
        server.Start();

        var hardStop = new HardStopCommand(server);
        Assert.Throws<InvalidOperationException>(() => hardStop.Execute());

        server.EnqueueCommand(new SoftStopCommand(server));
        server.Join();
    }
}