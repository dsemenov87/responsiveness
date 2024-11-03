using MathNet.Numerics.Statistics;

namespace responsiveness.CommonServices;

public sealed class StatsCalculator: IStatsCalculator
{
    public double Mean(IEnumerable<double> data) => data.Mean();

    public double RecalculateMean(double mean, int number, int val) =>
        (mean * (number - 1) + val) / number;

    public double StdDev(IEnumerable<double> data, double mean) => data.StandardDeviation();
        // data.Aggregate(0.0,
        //     (sum, d) => sum + (d - mean) * (d - mean),
        //     sum => Math.Sqrt(sum / data.Count()));

    public double Median(IEnumerable<double> data) => data.Median();
}