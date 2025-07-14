using System;
using System.Threading;
namespace task14;

public static class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        double total = 0.0;
        using var barrier = new Barrier(threadsNumber + 1);
        double segment = (b - a) / threadsNumber;

        for (int i = 0; i < threadsNumber; i++)
        {
            double start = a + i * segment;
            double end = (i == threadsNumber - 1) ? b : start + segment;

            new Thread(() =>
            {
                double partialSum = 0;
                for (double x = start; x < end; x += step)
                {
                    double currentStep = Math.Min(step, end - x);
                    partialSum += (function(x) + function(x + currentStep)) * currentStep / 2;
                }

                Interlocked.Exchange(ref total, total + partialSum);
                barrier.SignalAndWait();
            }).Start();
        }

        barrier.SignalAndWait();
        return total;
    }
}
