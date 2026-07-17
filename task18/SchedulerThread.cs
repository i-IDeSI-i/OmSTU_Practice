using System.Collections.Concurrent;
using task17;

namespace task18;

public class SchedulerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly RoundRobinScheduler _scheduler = new();
    private readonly Thread _thread;
    private volatile bool _stop = false;

    public SchedulerThread()
    {
        _thread = new Thread(Loop);
    }

    public bool IsAlive => _thread.IsAlive;
    public void Start() => _thread.Start();
    public void EnqueueCommand(ICommand command) => _queue.Add(command);
    public void Stop() => _stop = true;
    public void Join() => _thread.Join();

    private void Loop()
    {
        while (!_stop)
        {

            while (_queue.TryTake(out var newCmd))
            {
                if (newCmd is ILongRunningCommand)
                    _scheduler.Add(newCmd);
                else
                    newCmd.Execute();
            }

            if (_scheduler.HasCommand())
            {
                var cmd = _scheduler.Select();
                cmd.Execute();

                if (cmd is ILongRunningCommand lr && lr.IsCompleted)
                    _scheduler.Remove(cmd);
            }
            else
            {
                if (_queue.TryTake(out var waited, Timeout.Infinite))
                {
                    if (waited is ILongRunningCommand)
                        _scheduler.Add(waited);
                    else
                        waited.Execute();
                }
            }
        }
    }
}