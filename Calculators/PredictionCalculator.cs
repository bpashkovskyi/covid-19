using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19.Calculators
{
    using System.ComponentModel.DataAnnotations;

    using Covid19.Models;

    public class PredictionCalculator
    {
        private const string ConfirmedCsvUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Confirmed.csv";
        private const string DeathCsvUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Deaths.csv";
        private const string RecoveredCsvUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Recovered.csv";
        private const int PredictToCases = 2000000;
        private const int PredictFromCases = 10000;


        private TimeSeriesDataReader timeSeriesDataReader;
        private RegressionCalculator regressionCalculator;

        public PredictionCalculator()
        {
            this.timeSeriesDataReader = new TimeSeriesDataReader();
            this.regressionCalculator = new RegressionCalculator();
        }

        public TimeSeries CreateTimeSeries()
        {
            var confirmedTimeSeries = this.CreateTimeSeries(ConfirmedCsvUrl);
            return confirmedTimeSeries;
        }

        private TimeSeries CreateTimeSeries(string casesTimeSeriesUrl)
        {
            var realDailyData = this.timeSeriesDataReader.GetDailyCasesOutsideChina(casesTimeSeriesUrl);
            var realDataUsedForPrediction = realDailyData.Where(day => day.TotalCases > PredictFromCases).ToList();

            var exponentialRegressionCoefficients = this.regressionCalculator.CalculateExponentialRegressionCoefficients(realDataUsedForPrediction.Select(day => (day.DayNumber, day.TotalCases.Value)).ToArray());

            var timeSeries = new TimeSeries
            {
                DaysData = realDataUsedForPrediction,
                ExponentialRegressionCoefficients = exponentialRegressionCoefficients,
            };

            var predictedTimeSeries = this.CreatePredictedTimeSeries(timeSeries);
            return predictedTimeSeries;
        }

        private DailyData PredictDay(long dayNumber, string dateString, DateTime? date, long? dayTotalCases, ExponentialRegressionCoefficients coefficients)
        {
            var prediction = (long)Math.Round(this.regressionCalculator.Exp(coefficients.A, coefficients.B, dayNumber));
            var predictionFrom = (long)Math.Round(prediction / (1 + coefficients.RegressionError));
            var predictionTo = (long)Math.Round(prediction * (1 + coefficients.RegressionError));

            var dailyData = new DailyData
            {
                DayNumber = dayNumber,
                DateString = !string.IsNullOrEmpty(dateString) ? dateString : date?.ToShortDateString(),
                Date = date ?? (string.IsNullOrEmpty(dateString) ? (DateTime?)null : DateTime.Parse(dateString)),
                PredictedTotalCasesFrom = predictionFrom,
                PredictedTotalCasesTo = predictionTo,
                TotalCases = dayTotalCases
            };

            return dailyData;
        }

        private TimeSeries CreatePredictedTimeSeries(TimeSeries timeSeries)
        {
            var days = timeSeries.DaysData;

            var predictedDays = days.Select(day => this.PredictDay(day.DayNumber, day.DateString, null, day.TotalCases, timeSeries.ExponentialRegressionCoefficients)).ToList();

            while (predictedDays.Last().PredictedTotalCasesFrom < PredictToCases)
            {
                var lastPredictedDay = predictedDays.Last();
                predictedDays.Add(this.PredictDay(lastPredictedDay.DayNumber + 1, null, lastPredictedDay.Date.Value.AddDays(1), null, timeSeries.ExponentialRegressionCoefficients));
            }

            foreach (var day in predictedDays.Where(day => day.TotalCases.HasValue))
            {
                var previousDayIndex = predictedDays.IndexOf(day) - 1;
                var previousDay = previousDayIndex < 0 ? null : predictedDays[previousDayIndex];

                day.NewCases = previousDay == null ? 0 : day.TotalCases - previousDay.TotalCases ;
                day.IncreaseRate = previousDay == null ? 1 : (double)day.TotalCases / (double)previousDay.TotalCases;
            }

            foreach (var day in predictedDays)
            {
                var previousDayIndex = predictedDays.IndexOf(day) - 1;
                var previousDay = previousDayIndex < 0 ? null : predictedDays[previousDayIndex];

                day.PredictedNewCasesFrom = previousDay == null ? 0 : day.PredictedTotalCasesFrom - previousDay.PredictedTotalCasesFrom;
                day.PredictedNewCasesTo = previousDay == null ? 0 : day.PredictedTotalCasesTo - previousDay.PredictedTotalCasesTo;
            }

            return new TimeSeries
            {
                DaysData = predictedDays,
                ExponentialRegressionCoefficients = timeSeries.ExponentialRegressionCoefficients
            };
        }
    }
}
