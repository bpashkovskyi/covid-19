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
        private const int DaysToPredict = 130;
        private readonly DateTime dayOne = new DateTime(2020, 01, 22);

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
            var exponentialRegressionCoefficients = this.regressionCalculator.CalculateExponentialRegressionCoefficients(realDailyData.Where(day => day.TotalCases.HasValue && day.TotalCases != 0).Select(day => (day.DayNumber, day.TotalCases.Value)).ToArray());

            var timeSeries = new TimeSeries
            {
                DaysData = realDailyData,
                ExponentialRegressionCoefficients = exponentialRegressionCoefficients,
            };

            this.EnrichWithPredictedData(timeSeries);

            return timeSeries;
        }

        private void EnrichWithPredictedData(TimeSeries timeSeries)
        {
            for (var dayNumber = 1; dayNumber <= DaysToPredict; dayNumber++)
            {
                var predictedDailyData = new DailyData
                {
                    Date = this.dayOne.AddDays(dayNumber - 1),
                    DayNumber = dayNumber,
                    PredictionFrom = (long)Math.Round(this.regressionCalculator.Exp(timeSeries.ExponentialRegressionCoefficients.A, timeSeries.ExponentialRegressionCoefficients.B, dayNumber)),
                };
                predictedDailyData.PredictionTo = (long)(predictedDailyData.PredictionFrom * (1 + timeSeries.ExponentialRegressionCoefficients.RegressionError));

                if (dayNumber <= timeSeries.DaysData.Count)
                {
                    timeSeries.DaysData[dayNumber - 1].PredictionFrom = predictedDailyData.PredictionFrom;
                    timeSeries.DaysData[dayNumber - 1].PredictionTo = predictedDailyData.PredictionTo;
                    timeSeries.DaysData[dayNumber - 1].Date = predictedDailyData.Date;
                    timeSeries.DaysData[dayNumber - 1].NewCases = dayNumber == 1 ? timeSeries.DaysData[dayNumber - 1].TotalCases : timeSeries.DaysData[dayNumber - 1].TotalCases - timeSeries.DaysData[dayNumber - 2].TotalCases;
                    timeSeries.DaysData[dayNumber - 1].IncreaseRate = dayNumber == 1 ? 1 : (double)timeSeries.DaysData[dayNumber - 1].TotalCases / (double)timeSeries.DaysData[dayNumber - 2].TotalCases;
                }
                else
                {
                    timeSeries.DaysData.Add(predictedDailyData);
                }
            }
        }
    }
}
