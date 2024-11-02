using MathNet.Numerics.Statistics;

namespace responsiveness.CommonServices;

public sealed class StatsCalculator: IStatsCalculator
{
    public double Mean(double[] data) => data.Mean();

    public double RecalculateMean(double mean, int number, int val) =>
        (mean * (number - 1) + val) / number;

    public double StandartDeviation(double[] data, double mean) =>
        data.Aggregate(0.0,
            (sum, d) => sum + (d - mean) * (d - mean),
            sum => Math.Sqrt(sum / data.Length));

    public double Median(double[] data) => data.Median();
}