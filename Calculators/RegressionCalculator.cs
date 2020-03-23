namespace Covid19.Calculators
{
    using System;
    using System.Linq;

    using Covid19.Models;

    public class RegressionCalculator
    {
        public ExponentialRegressionCoefficients CalculateExponentialRegressionCoefficients((long x, long y)[] realValues)
        {
            double b = this.CalculateB(realValues);
            double a = this.CalculateA(b, realValues);
            double error = this.CalculateError(a, b, realValues);

            return new ExponentialRegressionCoefficients { A = a, B = b, RegressionError = error };
        }

        private double CalculateB((long x, long y)[] realValues)
        {
            double n = realValues.Length;

            double t1 = n * realValues.Sum(v => v.x * Math.Log(v.y));
            double t2 = realValues.Sum(v => v.x) * realValues.Sum(v => Math.Log(v.y));
            double t3 = n * realValues.Sum(v => Math.Pow(v.x, 2));
            double t4 = Math.Pow(realValues.Sum(v => v.x), 2);

            double b = (t1 - t2) / (t3 - t4);

            return b;
        }

        private double CalculateA(double b, (long x, long y)[] realValues)
        {
            double n = realValues.Length;

            double t1 = realValues.Sum(v => Math.Log(v.y));
            double t2 = realValues.Sum(v => v.x);

            double a = (1 / n) * t1 - (b / n) * t2;

            return a;
        }

        private double CalculateError(double a, double b, (long x, long y)[] realValues)
        {
            double n = realValues.Length;

            double error = (1 / n) * realValues.Sum(v => Math.Abs(v.y - Exp(a, b, v.x)) / v.y);

            return error;
        }

        public double Exp(double a, double b, double x)
        {
            return Math.Exp(a + b * x);
        }
    }
}
