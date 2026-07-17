using System.Collections.Concurrent;

namespace task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly Thread _thread;

    private volatile bool _hardStop = false;
    private volatile bool _softStop = false;

    private readonly Action<ICommand, Exception>? _exceptionHandler;

    public ServerThread(Action<ICommand, Exception>? exceptionHandler = null)
    {
        _exceptionHandler = exceptionHandler;
        _thread = new Thread(Loop);
    }

    public int ManagedThreadId => _thread.ManagedThreadId;

    public bool IsAlive => _thread.IsAlive;

    public void Start() => _thread.Start();

    public void EnqueueCommand(ICommand command) => _queue.Add(command);

    public void Join() => _thread.Join();

    internal void SignalHardStop() => _hardStop = true;
    internal void SignalSoftStop() => _softStop = true;

    private void Loop()
    {
        while (true)
        {

            if (_hardStop)
                break;

            if (_softStop && _queue.Count == 0)
                break;

            ICommand command;
            try
            {
                if (!_queue.TryTake(out command!, Timeout.Infinite))
                    continue;
            }
            catch (InvalidOperationException)
            {
                break;
            }

            try
            {
                command.Execute();
            }
            catch (Exception ex)
            {
                _exceptionHandler?.Invoke(command, ex);
            }
        }
    }
}