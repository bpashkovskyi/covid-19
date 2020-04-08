namespace Covid19.Calculators
{
    using System;
    using System.Linq;

    using DomainModels;

    public class ExponentialRegressionCalculator
    {
        public ExponentialRegressionCoefficients CalculateExponentialRegression((long x, long y)[] realValues)
        {
            var b = CalculateExpB(realValues);
            var a = CalculateExpA(b, realValues);
            var error = CalculateExpError(a, b, realValues);

            return new ExponentialRegressionCoefficients { A = a, B = b, RegressionError = error };
        }

        private double CalculateExpB((long x, long y)[] realValues)
        {
            double n = realValues.Length;

            var t1 = n * realValues.Sum(v => v.x * Math.Log(v.y));
            var t2 = realValues.Sum(v => v.x) * realValues.Sum(v => Math.Log(v.y));
            var t3 = n * realValues.Sum(v => Math.Pow(v.x, 2));
            var t4 = Math.Pow(realValues.Sum(v => v.x), 2);

            var b = (t1 - t2) / (t3 - t4);

            return b;
        }

        private double CalculateExpA(double b, (long x, long y)[] realValues)
        {
            double n = realValues.Length;

            var t1 = realValues.Sum(v => Math.Log(v.y));
            double t2 = realValues.Sum(v => v.x);

            var a = (1 / n) * t1 - (b / n) * t2;

            return a;
        }

        private double CalculateExpError(double a, double b, (long x, long y)[] realValues)
        {
            ////double n = realValues.Length;

            ////var error = (1 / n) * realValues.Sum(v => Math.Abs(v.y - this.Exp(a, b, v.x)) / v.y);

            return 0;
        }

        public double Exp(ExponentialRegressionCoefficients coefficients, double x)
        {
            return Math.Exp(coefficients.A + coefficients.B * x);
        }
    }
}
