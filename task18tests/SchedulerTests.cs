using Xunit;

public class ServerThreadTests
{
    [Fact]
    public void TestSchedulerWithLongRunningCommands()
    {
        var server = new ServerThread();
        server.Start();

        int executionCount = 0;
        var longCommand = new ServerThread.LongRunningCommand(() => executionCount++);

        server.Enqueue(longCommand);
        Thread.Sleep(100);

        Assert.True(executionCount > 0);

        server.Enqueue(new SoftStopCommand(server));
        server.WaitForCompletion();
    }

    [Fact]
    public void TestRoundRobinSchedulerOrder()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);

        int counter1 = 0;
        int counter2 = 0;

        var cmd1 = new TestCommand(() => counter1++);
        var cmd2 = new TestCommand(() => counter2++);

        scheduler.Add(cmd1);
        scheduler.Add(cmd2);

        scheduler.Select().Execute();
        scheduler.Select().Execute();
        scheduler.Select().Execute();

        Assert.Equal(2, counter1);
        Assert.Equal(1, counter2);
    }

    private class TestCommand : ICommand
    {
        private readonly Action _action;

        public TestCommand(Action action)
        {
            _action = action;
        }

        public void Execute() => _action();
    }
}
