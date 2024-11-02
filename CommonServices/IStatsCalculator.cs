namespace responsiveness.CommonServices;

public interface IStatsCalculator
{
    double Mean(double[] data);
    
    double RecalculateMean(double mean, int number, int val);

    double StandartDeviation(double[] data, double mean);

    double Median(double[] data);
}