using System.Collections.Concurrent;
using System.Diagnostics;

public interface ICommand
{
    void Execute();
}

public interface IScheduler
{
    bool HasCommand();
    ICommand Select();
    void Add(ICommand cmd);
}

public class RoundRobinScheduler : IScheduler
{
    private readonly Queue<ICommand> _commands = new();
    
    public bool HasCommand() => _commands.Count > 0;
    
    public ICommand Select()
    {
        var command = _commands.Dequeue();
        _commands.Enqueue(command);
        return command;
    }
    
    public void Add(ICommand cmd) => _commands.Enqueue(cmd);
}

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _commands = new();
    private readonly IScheduler _scheduler;
    private volatile bool _isRunning;
    private Thread? _thread;

    public ServerThread() : this(new RoundRobinScheduler())
    {
    }

    public ServerThread(IScheduler scheduler)
    {
        _scheduler = scheduler;
    }

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
            if (_commands.TryTake(out var command))
            {
                command.Execute();
                continue;
            }

            if (_scheduler.HasCommand())
            {
                var schedulerCommand = _scheduler.Select();
                schedulerCommand.Execute();
                continue;
            }

            Thread.Sleep(10);
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

    public class LongRunningCommand : ICommand
    {
        private readonly Action _action;
        private int _executionCount;
        private readonly List<long> _executionTimes = new List<long>();

        public LongRunningCommand(Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            var sw = Stopwatch.StartNew();
            _action();
            _executionCount++;
            sw.Stop();
            _executionTimes.Add(sw.ElapsedMilliseconds);
        }

        public int ExecutionCount => _executionCount;
        public IReadOnlyList<long> ExecutionTimes => _executionTimes.AsReadOnly();
    }
    
    public class TestCommand : ICommand
    {
        private readonly int _id;
        private int _counter = 0;

        public TestCommand(int id)
        {
            _id = id;
        }

        public int Id => _id;
        public int Counter => _counter;

        public void Execute()
        {
            Console.WriteLine($"Поток {_id} вызов {++_counter}");
        }
    }
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
