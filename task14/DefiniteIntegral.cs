namespace task14;
public static class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        double[] localSums = new double[threadsNumber];
        double chunkSize = (b - a) / threadsNumber;

        Parallel.For(0, threadsNumber, i =>
        {
            double start = a + i * chunkSize;
            double end = (i == threadsNumber - 1) ? b : start + chunkSize;
            double sum = 0.0;
            for (double x = start; x < end; x += step)
            {
                double dx = Math.Min(step, end - x);
                sum += (function(x) + function(x + dx)) * dx / 2;
            }
            localSums[i] = sum;
        });

        double total = 0;
        for (int i = 0; i < threadsNumber; i++)
            total += localSums[i];
        return total;
    }

    public static double SolveSingleThread(double a, double b, Func<double, double> function, double step)
    {
        double sum = 0.0;
        for (double x = a; x < b; x += step)
        {
            double dx = Math.Min(step, b - x);
            sum += (function(x) + function(x + dx)) * dx / 2;
        }
        return sum;
    }
}
