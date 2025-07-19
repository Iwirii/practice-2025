using ScottPlot;

class Program
{
    static void Main()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);

        int commandsCount = 5;

        var commands = new List<ServerThread.LongRunningCommand>();

        for (int i = 0; i < commandsCount; i++)
        {
            int delay = 50 + i * 5;
            commands.Add(new ServerThread.LongRunningCommand(() => Thread.Sleep(delay)));
        }

        server.Start();

        foreach (var cmd in commands)
            server.Enqueue(cmd);

        Thread.Sleep(2000);

        server.Enqueue(new SoftStopCommand(server));
        server.WaitForCompletion();

        var plt = new Plot();
        double[] commandNumbers = Enumerable.Range(1, commandsCount).Select(x => (double)x).ToArray(); 
        double[] avgTimes = commands.Select(c => c.ExecutionTimes.Any() ? c.ExecutionTimes.Average() : 0).ToArray();

        plt.Title("Среднее время выполнения команд");
        plt.XLabel("Среднее время (мс)");
        plt.YLabel("Номер команды");
        
        var scatter = plt.Add.Scatter(avgTimes, commandNumbers);
        
        plt.SavePng("graph.png", 600, 300);

        string report = "Отчет\n\n";
        report += "Номер команды | Среднее время (мс)\n";

        for (int i = 0; i < commands.Count; i++)
        {
            report += $"{i + 1,13} | {commands[i].ExecutionTimes.Average(),18:F2}\n";
        }

        File.WriteAllText("report.txt", report);
    }
}