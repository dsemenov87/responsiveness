namespace responsiveness.Abstractions;

public interface IStatsCalculator
{
    double Mean(IEnumerable<double> data);
    
    double RecalculateMean(double mean, int number, int val);

    double StdDev(IEnumerable<double> data, double mean);

    double Median(IEnumerable<double> data);
}