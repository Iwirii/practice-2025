using ScottPlot;
using System.Diagnostics;
using System.Linq;
using task14;

class Optimizer
{
    static void Main()
    {
        double a = -100;
        double b = 100;
        Func<double, double> f = Math.Sin;
        int tests = 10;
        double eps = 1e-4;
        double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
        int[] threads = { 1, 2, 3, 4, 5, 6, 7, 8 };

        double step = FindOptimalStep(a, b, f, steps, eps);
        double[] times = TestThreads(a, b, f, step, threads, tests);
        (int bestThreads, double bestTime) = FindBestResult(threads, times);
        double singleTime = TestSingleThread(a, b, f, step, tests);

        SaveResults(step, bestThreads, bestTime, singleTime, threads, times);
    }

    static double FindOptimalStep(double a, double b, Func<double, double> f, double[] steps, double eps)
    {
        foreach (var step in steps)
        {
            if (Math.Abs(DefiniteIntegral.SolveSingleThread(a, b, f, step)) <= eps)
                return step;
        }
        return steps.Last();
    }

    static double[] TestThreads(double a, double b, Func<double, double> f, double step, int[] threads, int tests)
    {
        double[] times = new double[threads.Length];
        for (int i = 0; i < threads.Length; i++)
        {
            var sw = Stopwatch.StartNew();
            for (int j = 0; j < tests; j++)
                DefiniteIntegral.Solve(a, b, f, step, threads[i]);
            times[i] = sw.Elapsed.TotalMilliseconds / tests;
        }
        return times;
    }

    static (int bestThreads, double bestTime) FindBestResult(int[] threads, double[] times)
    {
        int bestIndex = 0;
        for (int i = 1; i < times.Length; i++)
            if (times[i] < times[bestIndex]) bestIndex = i;
        return (threads[bestIndex], times[bestIndex]);
    }

    static double TestSingleThread(double a, double b, Func<double, double> f, double step, int tests)
    {
        var sw = Stopwatch.StartNew();
        for (int j = 0; j < tests; j++)
            DefiniteIntegral.SolveSingleThread(a, b, f, step);
        return sw.Elapsed.TotalMilliseconds / tests;
    }

    static void SaveResults(double step, int bestThreads, double bestTime,
                         double singleTime, int[] threads, double[] times)
    {
        var plt = new Plot();
        plt.Add.Scatter(times, threads.Select(x => (double)x).ToArray());
        plt.Title("Время выполнения");
        plt.XLabel("Время (мс)");
        plt.YLabel("Потоки");
        plt.SavePng("graph.png", 800, 600);

        string report =
            $"Шаг: {step}\n" +
            $"Оптимальное число потоков: {bestThreads}\n" +
            $"Многопоток: {bestTime:F4} мс\n" +
            $"Однопоток: {singleTime:F4} мс\n" +
            $"Разница: {(singleTime - bestTime) / singleTime * 100:F2}%";

        File.WriteAllText("report.txt", report);
    }
}
