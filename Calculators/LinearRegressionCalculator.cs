using System;
using System.Linq;
using Covid19.DomainModels;

namespace Covid19.Calculators
{
    public class LinearRegressionCalculator
    {
        public LinearRegressionCoefficients CalculateLinearRegression((double x, double y)[] realValues)
        {
            var b = CalculateLinearB(realValues);
            var a = CalculateLinearA(realValues);
            var error = CalculateLinearError(a, b, realValues);

            return new LinearRegressionCoefficients { A = a, B = b, RegressionError = error };
        }

        private double CalculateLinearError(double a, double b, (double x, double y)[] realValues)
        {
            double n = realValues.Length;
            var coefficients = new LinearRegressionCoefficients { A = a, B = b };

            var error = 1 / n * realValues.Sum(v =>  Math.Abs((v.y - this.Linear(coefficients, v.x)) / v.y));

            return error;

        }

        private double CalculateLinearA((double x, double y)[] realValues)
        {
            double n = realValues.Length;

            var t1 = realValues.Sum(v => v.x) * realValues.Sum(v => v.y);
            var t2 = n * realValues.Sum(v => v.x * v.y);
            var t3 = Math.Pow(realValues.Sum(v => v.x), 2);
            var t4 = n * realValues.Sum(v => Math.Pow(v.x, 2));

            var a = (t1 - t2) / (t3 - t4);

            return a;
        }

        private double CalculateLinearB((double x, double y)[] realValues)
        {
            double n = realValues.Length;

            var t1 = realValues.Sum(v => v.x) * realValues.Sum(v => v.x * v.y);
            var t2 = realValues.Sum(v => Math.Pow(v.x, 2)) * realValues.Sum(v => v.y);
            var t3 = Math.Pow(realValues.Sum(v => v.x), 2);
            var t4 = n * realValues.Sum(v => Math.Pow(v.x, 2));

            var b = (t1 - t2) / (t3 - t4);

            return b;
        }

        public double Linear(LinearRegressionCoefficients coefficients, double x)
        {
            return coefficients.A * x + coefficients.B;
        }
    }
}