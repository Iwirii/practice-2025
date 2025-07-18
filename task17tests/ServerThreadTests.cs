using Xunit;
namespace task17tests;

public class ServerThreadTests
{
        private class TestCommand : ICommand
    {
        private readonly Action _action;
        public TestCommand(Action action) => _action = action;
        public void Execute() => _action();
    }
    
    [Fact]
    public void HardStop_StopsImmediately()
    {
        var server = new ServerThread();
        server.Start();

        bool commandExecuted = false;
        server.Enqueue(new HardStopCommand(server));
        server.Enqueue(new TestCommand(() => commandExecuted = true));

        server.WaitForCompletion();
        Assert.False(commandExecuted);
    }

    [Fact]
    public void SoftStop_ProcessesAllCommands()
    {
        var server = new ServerThread();
        server.Start();

        bool commandExecuted = false;
        server.Enqueue(new TestCommand(() => commandExecuted = true));
        server.Enqueue(new SoftStopCommand(server));

        server.WaitForCompletion();
        Assert.True(commandExecuted);
    }

    [Fact]
    public void StopCommands_ThrowIfNotFromOwnThread()
    {
        var server = new ServerThread();
        server.Start();

        Assert.Throws<InvalidOperationException>(() => server.HardStop());
        Assert.Throws<InvalidOperationException>(() => server.SoftStop());

        server.Enqueue(new HardStopCommand(server));
        server.WaitForCompletion();
    }

     [Fact]
    public void SoftStop_WaitsForAllCommands()
    {
        var server = new ServerThread();
        server.Start();
        
        int counter = 0;
        server.Enqueue(new TestCommand(() => counter++));
        server.Enqueue(new TestCommand(() => counter++));
        server.Enqueue(new SoftStopCommand(server));
        
        server.WaitForCompletion();
        Assert.Equal(2, counter); 
    }
}