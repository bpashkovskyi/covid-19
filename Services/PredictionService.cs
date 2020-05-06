namespace Covid19.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Covid19.ApplicationModels;
    using Covid19.Models.Entities;

    using MathNet.Numerics;

    public class PredictionService
    {
        private readonly TimeSeriesReadService timeSeriesReadService;

        public PredictionService()
        {
            this.timeSeriesReadService = new TimeSeriesReadService();
        }

        public PredictionOutputModel CreatePredictionTimeSeries(PredictionInputModel predictionInputModel)
        {
            var readTimeSeriesInputModel = new TimeSeriesReadInputModel
            {
                Countries = predictionInputModel.Settings.Countries.Split(',').ToList(),
                CountrySearchType = predictionInputModel.Settings.CountrySearchType,
                TimeSeriesType = predictionInputModel.Settings.TimeSeriesType
            };

            var timeSeries = this.timeSeriesReadService.ReadTimeSeries(readTimeSeriesInputModel).TimeSeries;
            var filteredTimeSeries = this.FilterTimeSeries(timeSeries, predictionInputModel.Settings);

            this.AdjustRealDaysData(filteredTimeSeries);
            var regressionFunction = this.CalculateRegressionFunction(filteredTimeSeries);
            var predictionTimeSeries = this.CreatePredictionTimeSeries(filteredTimeSeries, regressionFunction);

            return new PredictionOutputModel
            {
                TimeSeries = predictionTimeSeries,
                Settings = predictionInputModel.Settings,
            };
        }


        private TimeSeries FilterTimeSeries(TimeSeries timeSeries, PredictionSettings predictionSettings)
        {
            return new TimeSeries { DaysData = timeSeries.DaysData.Skip(timeSeries.DaysData.Count - predictionSettings.UseLastDays).ToList() };
        }

        private void AdjustRealDaysData(TimeSeries timeSeries)
        {
            foreach (var day in timeSeries.DaysData)
            {
                var previousDay = timeSeries.DaysData.GetPreviousElement(day, 1);

                day.NewCases = previousDay == null ? 0 : day.TotalCases - previousDay.TotalCases;

                var previousWeekDay = timeSeries.DaysData.GetPreviousElement(day, 7);
                day.WeeklyNewCases = previousWeekDay == null ? 0 : day.TotalCases - previousWeekDay.TotalCases;
            }
        }

        private Func<double, double> CalculateRegressionFunction(TimeSeries timeSeries)
        {
            var daysDataForPrediction = timeSeries.DaysData
                .Skip(1).ToArray();

            var xData = daysDataForPrediction.Select(dayData => (double)dayData.DayNumber).ToArray();
            var yData = daysDataForPrediction.Select(dayData => (double)dayData.NewCases).ToArray();

            var regressionFunction = Fit.PolynomialFunc(xData, yData, 2);
            return regressionFunction;
        }

        private PredictionTimeSeries CreatePredictionTimeSeries(TimeSeries timeSeries, Func<double, double> regressionFunction)
        {
            var predictionDaysData = new List<PredictionDayData> { timeSeries.DaysData.First().ToPredictionDayData() };

            while (predictionDaysData.Count < (timeSeries.DaysData.Count + 14))
            {
                var lastPredictionDay = predictionDaysData.Last();

                var realDay = timeSeries.DaysData.FirstOrDefault(day => day.DayNumber == (lastPredictionDay.DayNumber + 1));

                var newPredictionDay = new PredictionDayData
                {
                    Date = lastPredictionDay.Date.AddDays(1),
                    DayNumber = lastPredictionDay.DayNumber + 1,
                    TotalCases = realDay?.TotalCases,
                    NewCases = realDay?.NewCases,
                    WeeklyNewCases = realDay?.WeeklyNewCases
                };

                newPredictionDay.PredictionNewCases = (long)regressionFunction(newPredictionDay.DayNumber);
                newPredictionDay.PredictionTotalCases = lastPredictionDay.PredictionTotalCases + newPredictionDay.PredictionNewCases;

                predictionDaysData.Add(newPredictionDay);
            }

            predictionDaysData.Remove(predictionDaysData.Last());

            return new PredictionTimeSeries
            {
                DaysData = predictionDaysData.Where(day => day.PredictionNewCases > 0).ToList()
            };
        }
    }
}