using System;
using System.Linq;

namespace Covid19.Calculators
{
    using System.Collections.Generic;

    using ApplicationModels;
    using DomainModels;

    public class PredictionCalculator
    {
        private readonly TimeSeriesReader timeSeriesReader;
        private readonly LinearRegressionCalculator regressionCalculator;

        public PredictionCalculator()
        {
            timeSeriesReader = new TimeSeriesReader();
            regressionCalculator = new LinearRegressionCalculator();
        }

        public PredictionOutputModel CreatePredictionTimeSeries(PredictionInputModel predictionInputModel)
        {
            var readTimeSeriesInputModel = new TimeSeriesReadInputModel
            {
                Countries = predictionInputModel.Countries,
                CountrySearchType = predictionInputModel.CountrySearchType,
                TimeSeriesType = predictionInputModel.TimeSeriesType
            };

            var timeSeries = timeSeriesReader.ReadTimeSeries(readTimeSeriesInputModel).TimeSeries;
            var filteredTimeSeries = this.FilterTimeSeries(timeSeries, predictionInputModel);

            this.EnrichWithNewCasesAndIncreaseRate(filteredTimeSeries, predictionInputModel);
            var coefficients = this.CalculateRegression(filteredTimeSeries, predictionInputModel);
            var predictionTimeSeries = this.CreatePredictionTimeSeries(filteredTimeSeries, predictionInputModel, coefficients);

            return new PredictionOutputModel { TimeSeries = predictionTimeSeries };
        }

        private TimeSeries FilterTimeSeries(TimeSeries timeSeries, PredictionInputModel predictionInputModel)
        {
            return new TimeSeries { DaysData = timeSeries.DaysData.Skip(timeSeries.DaysData.Count - predictionInputModel.UseLastDays).ToList() };
        }

        private void EnrichWithNewCasesAndIncreaseRate(TimeSeries timeSeries, PredictionInputModel predictionInputModel)
        {
            foreach (var day in timeSeries.DaysData)
            {
                var previousDayIndex = timeSeries.DaysData.IndexOf(day) - 1;
                var previousDay = previousDayIndex < 0 ? null : timeSeries.DaysData[previousDayIndex];

                if (predictionInputModel.UseAverage)
                {
                    var nextDayIndex = timeSeries.DaysData.IndexOf(day) + 1;
                    var nextDay = nextDayIndex >= timeSeries.DaysData.Count ? null : timeSeries.DaysData[nextDayIndex];

                    if (previousDay != null && nextDay != null)
                    {
                        day.TotalCases = (long?)((previousDay.TotalCases + day.TotalCases + nextDay.TotalCases) / 3.0);
                    }
                }

                day.NewCases = previousDay == null ? 0 : day.TotalCases - previousDay.TotalCases;
                day.IncreaseRate = previousDay == null ? 1 : (double)day.TotalCases.Value / (double)previousDay.TotalCases.Value;
            }
        }

        private LinearRegressionCoefficients CalculateRegression(TimeSeries timeSeries, PredictionInputModel predictionInputModel)
        {
            var daysDataForPrediction = timeSeries.DaysData
                .Skip(1)
                .Select(day => ((double)day.DayNumber, day.IncreaseRate.Value))
                .ToArray();

            var x = string.Join(' ', daysDataForPrediction.Select(d => d.Item1));
            var y = string.Join(' ', daysDataForPrediction.Select(d => d.Value));

            var coefficients = regressionCalculator.CalculateLinearRegression(daysDataForPrediction);
            return coefficients;
        }

        private PredictedTimeSeries CreatePredictionTimeSeries(TimeSeries timeSeries, PredictionInputModel predictionInputModel, LinearRegressionCoefficients coefficients)
        {
            var firstDay = timeSeries.DaysData.First();
            firstDay.NewCases = 0;
            firstDay.IncreaseRate = 1;
            firstDay.PredictedIncreaseRate = regressionCalculator.Linear(coefficients, firstDay.DayNumber);
            firstDay.PredictedTotalCases = firstDay.TotalCases.Value;
            firstDay.PredictedNewCases = 0;

            var predictionDaysData = new List<DayData> { firstDay };

            while (predictionDaysData.Last().PredictedIncreaseRate >= 1)
            {
                var lastPredictedDay = predictionDaysData.Last();

                var realDay = timeSeries.DaysData.FirstOrDefault(day => day.DayNumber == lastPredictedDay.DayNumber + 1);
                var newPredictedDay = new DayData
                {
                    DateString = DateTime.Parse(lastPredictedDay.DateString).AddDays(1).ToShortDateString(),
                    DayNumber = lastPredictedDay.DayNumber + 1,
                    IncreaseRate = realDay?.IncreaseRate,
                    TotalCases = realDay?.TotalCases,
                    NewCases = realDay?.NewCases,
                };

                newPredictedDay.PredictedIncreaseRate = regressionCalculator.Linear(coefficients, newPredictedDay.DayNumber);
                newPredictedDay.PredictedTotalCases = (long)(lastPredictedDay.PredictedTotalCases * newPredictedDay.PredictedIncreaseRate);
                newPredictedDay.PredictedNewCases = newPredictedDay.PredictedTotalCases - lastPredictedDay.PredictedTotalCases;

                predictionDaysData.Add(newPredictedDay);
            }

            return new PredictedTimeSeries
            {
                DaysData = predictionDaysData,
                RegressionCoefficients = coefficients,
                TimeSeriesType = predictionInputModel.TimeSeriesType,
                UseLastDays = predictionInputModel.UseLastDays,
                Countries = string.Join(",", predictionInputModel.Countries),
                CountrySearchType = predictionInputModel.CountrySearchType,
                UseAverage = predictionInputModel.UseAverage,
                ViewType = predictionInputModel.ViewType,
            };
        }
    }
}
