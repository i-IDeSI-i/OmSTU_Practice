using Xunit;
using task17;
using task18;

public class SchedulerThreadTests
{
    private class CountingLongCommand : ILongRunningCommand
    {
        private int _done = 0;
        private readonly int _total;
        private readonly Action? _onStep;
        public CountingLongCommand(int total, Action? onStep = null)
        {
            _total = total;
            _onStep = onStep;
        }
        public bool IsCompleted => _done >= _total;
        public void Execute()
        {
            if (_done < _total)
            {
                _done++;
                _onStep?.Invoke();
            }
        }
        public int Done => _done;
    }

    private class SimpleCommand : ICommand
    {
        private readonly Action _action;
        public SimpleCommand(Action action) => _action = action;
        public void Execute() => _action();
    }

    [Fact]
    public void LongCommand_CompletesOverMultipleExecuteCalls()
    {
        var server = new SchedulerThread();
        server.Start();

        var cmd = new CountingLongCommand(10);
        server.EnqueueCommand(cmd);

        SpinWait.SpinUntil(() => cmd.IsCompleted, 2000);
        server.Stop();
        server.EnqueueCommand(new SimpleCommand(() => { }));
        server.Join();

        Assert.True(cmd.IsCompleted);
        Assert.Equal(10, cmd.Done);
    }

    [Fact]
    public void MultipleLongCommands_RunInterleaved()
    {
        var server = new SchedulerThread();
        server.Start();

        var a = new CountingLongCommand(50);
        var b = new CountingLongCommand(50);
        server.EnqueueCommand(a);
        server.EnqueueCommand(b);

        SpinWait.SpinUntil(() => a.IsCompleted && b.IsCompleted, 3000);
        server.Stop();
        server.EnqueueCommand(new SimpleCommand(() => { }));
        server.Join();

        Assert.True(a.IsCompleted);
        Assert.True(b.IsCompleted);
    }

    [Fact]
    public void ShortCommand_ExecutesImmediately()
    {
        var server = new SchedulerThread();
        server.Start();

        int value = 0;
        server.EnqueueCommand(new SimpleCommand(() => Interlocked.Exchange(ref value, 42)));

        SpinWait.SpinUntil(() => value == 42, 2000);
        server.Stop();
        server.EnqueueCommand(new SimpleCommand(() => { }));
        server.Join();

        Assert.Equal(42, value);
    }

    [Fact]
    public void RoundRobin_SelectsCommandsCyclically()
    {
        var scheduler = new RoundRobinScheduler();
        var c1 = new SimpleCommand(() => { });
        var c2 = new SimpleCommand(() => { });
        var c3 = new SimpleCommand(() => { });
        scheduler.Add(c1);
        scheduler.Add(c2);
        scheduler.Add(c3);

        Assert.Same(c1, scheduler.Select());
        Assert.Same(c2, scheduler.Select());
        Assert.Same(c3, scheduler.Select());
        Assert.Same(c1, scheduler.Select());
    }
}