using System.Diagnostics;
using ScottPlot;

namespace task18;

public class PerformanceAnalyzer
{
    public class TimedLongCommand : ILongRunningCommand
    {
        public int Id { get; }
        public List<long> StepTimes { get; } = new();
        private int _currentStep;
        private readonly int _totalSteps;
        private readonly Action<int> _action;

        public bool IsCompleted => _currentStep >= _totalSteps;

        public TimedLongCommand(int id, int steps, Action<int> action)
        {
            Id = id;
            _totalSteps = steps;
            _action = action;
        }

        public void Execute()
        {
            if (IsCompleted) return;

            var sw = Stopwatch.StartNew();
            _action(_currentStep);
            sw.Stop();

            StepTimes.Add(sw.ElapsedMilliseconds);
            _currentStep++;
        }
    }

    public static void Main()
    {
        var server = new ServerThread();
        var commands = new List<TimedLongCommand>();
        var random = new Random();

        for (int i = 0; i < 5; i++)
        {
            var cmd = new TimedLongCommand(
                id: i + 1,
                steps: 3,
                action: step => Thread.Sleep(random.Next(50, 200))
            );
            commands.Add(cmd);
            server.Enqueue(cmd);
        }

        server.Start();

        while (commands.Any(c => !c.IsCompleted))
            Thread.Sleep(100);

        server.Enqueue(new SoftStopCommand(server));
        server.WaitForCompletion();

        GenerateReport(commands);
    }

    private static void GenerateReport(List<TimedLongCommand> commands)
    {
        var plt = new Plot();
        plt.Title("Время выполнения команд");
        plt.YLabel("Номер команды");
        plt.XLabel("Время (мс)");

        for (int i = 0; i < commands.Count; i++)
        {
            var times = commands[i].StepTimes;
            plt.Add.Scatter(
                times.Select(t => (double)t).ToArray(),
                Enumerable.Repeat((double)i + 1, times.Count).ToArray()
            );
        }

        plt.SavePng("graph.png", 600, 300);

        var report = new System.Text.StringBuilder();
        report.AppendLine($"Всего команд: {commands.Count}");
        report.AppendLine($"Общее время выполнения: {commands.Sum(c => c.StepTimes.Sum())}мс");
        report.AppendLine("\nДетали по командам:");

        foreach (var cmd in commands.OrderBy(c => c.Id))
        {
            report.AppendLine($"\nКоманда {cmd.Id}:");
            report.AppendLine($"  Шагов: {cmd.StepTimes.Count}");
            report.AppendLine($"  Общее время: {cmd.StepTimes.Sum()}мс");
            report.AppendLine($"  Среднее время шага: {cmd.StepTimes.Average():F1}мс");
        }

        File.WriteAllText("report.txt", report.ToString());
    }
}
