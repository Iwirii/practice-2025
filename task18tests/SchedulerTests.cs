using Xunit;

public class SchedulerTests
{
    private class TestCommand : ICommand
    {
        private readonly Action _action;
        public TestCommand(Action action) => _action = action;
        public void Execute() => _action();
    }

    private class MultiStepCommand : ILongRunningCommand
    {
        private readonly Action<int> _stepAction;
        private int _currentStep;
        private readonly int _totalSteps;

        public bool IsCompleted => _currentStep >= _totalSteps;

        public MultiStepCommand(Action<int> stepAction, int totalSteps)
        {
            _stepAction = stepAction;
            _totalSteps = totalSteps;
            _currentStep = 0;
        }

        public void Execute()
        {
            if (!IsCompleted)
            {
                _stepAction(_currentStep);
                _currentStep++;
            }
        }
    }

    [Fact]
    public void LongRunningCommand_ExecutesInMultipleSteps()
    {
        var server = new ServerThread();
        server.Start();

        var steps = new List<int>();
        var command = new MultiStepCommand(step => steps.Add(step), 3);

        server.Enqueue(command);
        server.Enqueue(new SoftStopCommand(server));

        server.WaitForCompletion();

        Assert.Equal(3, steps.Count);
        Assert.Equal(0, steps[0]);
        Assert.Equal(1, steps[1]);
        Assert.Equal(2, steps[2]);
    }

    [Fact]
    public void MixedCommands_ExecuteInCorrectOrder()
    {
        var server = new ServerThread();
        server.Start();

        var results = new List<string>();
        var longCommand = new MultiStepCommand(step => results.Add($"Long{step}"), 2);

        server.Enqueue(new TestCommand(() => results.Add("Immediate1")));
        server.Enqueue(longCommand);
        server.Enqueue(new TestCommand(() => results.Add("Immediate2")));
        server.Enqueue(new SoftStopCommand(server));

        server.WaitForCompletion();

        Assert.Equal(4, results.Count);
        Assert.Equal("Immediate1", results[0]);
        Assert.Contains("Long0", results);
        Assert.Contains("Long1", results);
        Assert.Equal("Immediate2", results[3]);
    }
}
