using System.Collections.Concurrent;

public interface ICommand
{
    void Execute();
}

public interface ILongRunningCommand : ICommand
{
    bool IsCompleted { get; }
}

public interface IScheduler
{
    bool HasCommand();
    ICommand? Select();
    void Add(ICommand cmd);
}

public class RoundRobinScheduler : IScheduler
{
    private readonly ConcurrentQueue<ICommand> _commands = new();
    
    public bool HasCommand() => !_commands.IsEmpty;
    
    public ICommand? Select()
    {
        if (_commands.TryDequeue(out var command))
        {
            return command;
        }
        return null;
    }
    
    public void Add(ICommand cmd) => _commands.Enqueue(cmd);
}

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _commands = new();
    private readonly IScheduler _scheduler = new RoundRobinScheduler();
    private volatile bool _isRunning;
    private Thread? _thread;

    public void Start()
    {
        _isRunning = true;
        _thread = new Thread(Run);
        _thread.Start();
    }

    private void Run()
    {
        while (_isRunning)
        {
            if (_commands.TryTake(out var newCommand, TimeSpan.FromMilliseconds(100)))
            {
                if (newCommand is ILongRunningCommand longRunningCommand)
                {
                    _scheduler.Add(newCommand);
                }
                else
                {
                    newCommand.Execute();
                }
            }

            while (_scheduler.HasCommand())
            {
                var command = _scheduler.Select();
                if (command != null)
                {
                    command.Execute();
                    
                    if (command is ILongRunningCommand longRunningCommand && !longRunningCommand.IsCompleted)
                    {
                        _scheduler.Add(command);
                    }
                }
            }
        }
    }

    public void Enqueue(ICommand command) => _commands.Add(command);

    public void HardStop()
    {
        if (Thread.CurrentThread != _thread)
            throw new InvalidOperationException("HardStop может быть вызван только из потока сервера");
        _isRunning = false;
    }

    public void SoftStop()
    {
        if (Thread.CurrentThread != _thread)
            throw new InvalidOperationException("SoftStop может быть вызван только из потока сервера");
        _isRunning = false;
    }

    public void WaitForCompletion() => _thread?.Join();
}

public class HardStopCommand : ICommand
{
    private readonly ServerThread _server;
    public HardStopCommand(ServerThread server) => _server = server;
    public void Execute() => _server.HardStop();
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _server;
    public SoftStopCommand(ServerThread server) => _server = server;
    public void Execute() => _server.SoftStop();
}
