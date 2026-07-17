using task17;

namespace task18;

public class RoundRobinScheduler : IScheduler
{
    private readonly List<ICommand> _commands = new();
    private int _current = 0;
    private readonly object _lock = new();

    public void Add(ICommand cmd)
    {
        lock (_lock)
            _commands.Add(cmd);
    }

    public bool HasCommand()
    {
        lock (_lock)
            return _commands.Count > 0;
    }

    public ICommand Select()
    {
        lock (_lock)
        {
            if (_commands.Count == 0)
                throw new InvalidOperationException("В планировщике нет команд.");

            if (_current >= _commands.Count)
                _current = 0;

            var cmd = _commands[_current];
            _current++;
            return cmd;
        }
    }

    public void Remove(ICommand cmd)
    {
        lock (_lock)
        {
            int index = _commands.IndexOf(cmd);
            if (index >= 0)
            {
                _commands.RemoveAt(index);
                if (index < _current)
                    _current--;
            }
        }
    }
}