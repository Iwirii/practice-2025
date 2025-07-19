using ScottPlot;

class Program
{
    static void Main()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);

        var commands = Enumerable.Range(1, 5)
            .Select(id => new ServerThread.LongRunningCommand(() =>
            {
                Thread.Sleep(50 + id * 10);
            }))
            .ToList();

        server.Start();

        foreach (var cmd in commands)
        {
            for (int i = 0; i < 3; i++)
            {
                server.Enqueue(cmd);
            }
        }

        server.Enqueue(new HardStopCommand(server));
        server.WaitForCompletion();

        GenerateResults(commands);
    }

    static void GenerateResults(List<ServerThread.LongRunningCommand> commands)
    {
        double[] commandNumbers = Enumerable.Range(1, commands.Count).Select(x => (double)x).ToArray();
        double[] avgTimes = commands.Select(c => c.ExecutionTimes.Average()).ToArray();

        var plt = new Plot();
        plt.Title("Среднее время выполнения команд");
        plt.XLabel("Среднее время (мс)");
        plt.YLabel("Номер команды");

        var scatter = plt.Add.Scatter(avgTimes, commandNumbers);
        plt.SavePng("graph.png", 600, 300);

        string report = "ОТЧЕТ О ВЫПОЛНЕНИИ КОМАНД\n\n";
        report += "| Команда | Среднее | Минимум | Максимум | Всего |\n";
        report += "|---------|---------|---------|---------|-------|\n";

        for (int i = 0; i < commands.Count; i++)
        {
            var times = commands[i].ExecutionTimes;
            report += $"| {i + 1,6} | {times.Average(),7:F2} | {times.Min(),7:F2} | {times.Max(),7:F2} | {times.Sum(),6:F2} |\n";
        }

        var allTimes = commands.SelectMany(c => c.ExecutionTimes).ToList();
        report += "\nИтого:\n";
        report += $"Общее время выполнения: {allTimes.Sum():F2} мс\n";
        report += $"Среднее время всех команд: {allTimes.Average():F2} мс\n";
        report += $"Минимальное время: {allTimes.Min():F2} мс\n";
        report += $"Максимальное время: {allTimes.Max():F2} мс\n";

        File.WriteAllText("report.txt", report);
    }
}
