using System;
using System.Linq;
using System.Threading;
using Xunit;

public class Task19Tests
{
    [Fact]
    public void ShouldExecuteCommandsMultipleTimesBeforeHardStop()
    {
        var server = new ServerThread();
        server.Start();

        var commands = Enumerable.Range(1, 5)
            .Select(id => new ServerThread.TestCommand(id))
            .ToList();

        foreach (var cmd in commands)
        {
            for (int i = 0; i < 3; i++)
            {
                server.Enqueue(cmd);
            }
        }
        server.Enqueue(new HardStopCommand(server));
        server.WaitForCompletion();

        foreach (var cmd in commands)
        {
            Assert.Equal(3, cmd.Counter);
        }
    }
}
