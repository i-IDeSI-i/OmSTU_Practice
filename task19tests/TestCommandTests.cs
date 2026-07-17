using Xunit;
using task17;
using task18;
using task19;

public class TestCommandTests
{
    [Fact]
    public void TestCommand_CompletesAfterThreeCalls()
    {
        var cmd = new TestCommand(1, maxCalls: 3);

        Assert.False(cmd.IsCompleted);
        cmd.Execute();
        cmd.Execute();
        Assert.False(cmd.IsCompleted);
        cmd.Execute();
        Assert.True(cmd.IsCompleted);
        Assert.Equal(3, cmd.Counter);
    }

    [Fact]
    public void TestCommand_DoesNotExceedMaxCalls()
    {
        var cmd = new TestCommand(1, maxCalls: 3);

        for (int i = 0; i < 10; i++)
            cmd.Execute();

        Assert.Equal(3, cmd.Counter);
    }

    [Fact]
    public void Scheduler_RunsFiveCommandsToCompletion()
    {
        var server = new SchedulerThread();
        server.Start();

        var commands = new List<TestCommand>();
        for (int id = 1; id <= 5; id++)
        {
            var cmd = new TestCommand(id, maxCalls: 3);
            commands.Add(cmd);
            server.EnqueueCommand(cmd);
        }

        SpinWait.SpinUntil(() => commands.TrueForAll(c => c.IsCompleted), 3000);

        server.Stop();
        server.EnqueueCommand(new SimpleStopSignal());
        server.Join();

        Assert.All(commands, c => Assert.Equal(3, c.Counter));
        Assert.False(server.IsAlive);
    }
}