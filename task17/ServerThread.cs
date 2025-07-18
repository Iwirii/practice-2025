using System.Collections.Concurrent;

public interface ICommand
{
    void Execute();
}

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _commands = new();
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
            if (_commands.TryTake(out var command, 100))
            {
                command.Execute();
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