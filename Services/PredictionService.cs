namespace Covid19.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Covid19.ApplicationModels;
    using Covid19.Models.Entities;
    using Covid19.Models.Enums;
    using Covid19.Utilities;

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
            var casesTimeSeries = this.timeSeriesReadService.ReadTimeSeries(new TimeSeriesReadInputModel { TimeSeriesType = TimeSeriesType.Confirmed }).TimeSeriesByDay;
            var deathTimeSeries = this.timeSeriesReadService.ReadTimeSeries(new TimeSeriesReadInputModel { TimeSeriesType = TimeSeriesType.Death }).TimeSeriesByDay;

            var readTimeSeries = predictionInputModel.Settings.TimeSeriesType == TimeSeriesType.Confirmed ? casesTimeSeries : deathTimeSeries;

            var countryTimeSeriesByDay = this.GroupByCountries(readTimeSeries);
            var continentTimeSeriesByDay = this.GroupByContinents(readTimeSeries);

            var aggregatedTimeSeries = this.AggregateTimeSeries(countryTimeSeriesByDay, predictionInputModel.Settings);

            var regressionFunction = this.CalculateRegressionFunction(aggregatedTimeSeries);
            var predictionTimeSeries = this.CreatePredictionTimeSeries(aggregatedTimeSeries, regressionFunction);

            var countryTimeSeriesByLocation = countryTimeSeriesByDay.ToTimeSeriesByLocation();

            var selectedCounties = this.FilterSelectedCounties(countryTimeSeriesByLocation, predictionInputModel.Settings);
            var growingCountriesTimeSeries = this.GetGrowingCountriesTimeSeries(countryTimeSeriesByLocation);
            var decreasingCountriesTimeSeries = this.GetDecreasingCountriesTimeSeries(countryTimeSeriesByLocation);

            return new PredictionOutputModel
            {
                AggregatedTimeSeries = predictionTimeSeries,
                ContinentTimeSeries = continentTimeSeriesByDay.ToTimeSeriesByLocation(),
                GrowingCountriesTimeSeries = growingCountriesTimeSeries,
                SelectedCountriesTimeSeries = selectedCounties,
                DecreasingCountriesTimeSeries = decreasingCountriesTimeSeries,
                Settings = predictionInputModel.Settings
            };
        }

        private TimeSeriesByLocation FilterSelectedCounties(TimeSeriesByLocation countryTimeSeriesByLocation, PredictionSettings settings)
        {
            var locations = countryTimeSeriesByLocation.LocationsWithDayData.Where(location => settings.Countries.Contains(location.Country)).ToList();

            return new TimeSeriesByLocation { LocationsWithDayData = locations };
        }

        private TimeSeriesByLocation GetDecreasingCountriesTimeSeries(TimeSeriesByLocation countryTimeSeriesByLocation)
        {
            var locations = countryTimeSeriesByLocation.LocationsWithDayData.OrderBy(location => location.DayLocationData.Last().WeeklyNewCases - location.DayLocationData[location.DayLocationData.IndexOf(location.DayLocationData.Last()) - 30].WeeklyNewCases)
                .Take(5).ToList();

            return new TimeSeriesByLocation { LocationsWithDayData = locations };
        }

        private TimeSeriesByLocation GetGrowingCountriesTimeSeries(TimeSeriesByLocation countryTimeSeriesByLocation)
        {
            var locations = countryTimeSeriesByLocation.LocationsWithDayData.OrderByDescending(location => location.DayLocationData.Last().WeeklyNewCases - location.DayLocationData[location.DayLocationData.IndexOf(location.DayLocationData.Last()) - 30].WeeklyNewCases)
                .Take(5).ToList();

            return new TimeSeriesByLocation { LocationsWithDayData = locations };
        }

        private Func<double, double> CalculateRegressionFunction(TimeSeriesByDay timeSeriesByDay)
        {
            var daysDataForPrediction = timeSeriesByDay.DaysWithLocationData
                .Skip(1).ToArray();

            var xData = daysDataForPrediction.Select(dayData => (double)dayData.DayNumber).ToArray();
            var yData = daysDataForPrediction.Select(dayData => (double)dayData.NewCases).ToArray();

            var regressionFunction = Fit.PolynomialFunc(xData, yData, 2);
            return regressionFunction;
        }

        private TimeSeriesByDay AggregateTimeSeries(TimeSeriesByDay countriesTimeSeriesByDay, PredictionSettings predictionSettings)
        {
            var aggregatedTimeSeries = new TimeSeriesByDay();

            foreach (var day in countriesTimeSeriesByDay.DaysWithLocationData)
            {
                var aggregatedTimeSeriesDay = new DayWithLocationsData
                {
                    DayNumber = day.DayNumber,
                    Date = day.Date
                };

                foreach (var locationData in day.LocationsData)
                {
                    switch (predictionSettings.CountrySearchType)
                    {
                        case CountrySearchType.Inside when !string.IsNullOrEmpty(locationData.Country) && predictionSettings.Countries.Contains(locationData.Country):
                        case CountrySearchType.Outside when string.IsNullOrEmpty(locationData.Country) || !predictionSettings.Countries.Contains(locationData.Country):
                            aggregatedTimeSeriesDay.TotalCases += locationData.TotalCases;
                            break;
                    }
                }

                aggregatedTimeSeries.DaysWithLocationData.Add(aggregatedTimeSeriesDay);
            }

            this.AdjustRealDaysData(aggregatedTimeSeries);

            return aggregatedTimeSeries;
        }

        private TimeSeriesByDay GroupByCountries(TimeSeriesByDay readTimeSeriesByDay)
        {
            var countryTimeSeries = new TimeSeriesByDay();

            foreach (var countryDay in readTimeSeriesByDay.DaysWithLocationData)
            {
                var countriesData = countryDay.LocationsData.GroupBy(readData => readData.Country)
                    .Select(
                        countryData => new LocationData
                        {
                            Country = countryData.Key,
                            TotalCases = countryData.Sum(readData => readData.TotalCases)
                        }).ToList();

                var continentDay = new DayWithLocationsData
                {
                    DayNumber = countryDay.DayNumber,
                    Date = countryDay.Date,
                    LocationsData = countriesData
                };

                countryTimeSeries.DaysWithLocationData.Add(continentDay);
            }

            this.AdjustRealDaysData(countryTimeSeries);

            return countryTimeSeries;
        }

        private TimeSeriesByDay GroupByContinents(TimeSeriesByDay countiesTimeSeriesByDay)
        {
            var continentTimeSeries = new TimeSeriesByDay();

            foreach (var countryDay in countiesTimeSeriesByDay.DaysWithLocationData)
            {
                var continentsData = countryDay.LocationsData.GroupBy(countryData => countryData.Continent).Select(
                    continentData => new LocationData
                    {
                        Continent = continentData.Key,
                        TotalCases = continentData.Sum(countryData => countryData.TotalCases)
                    }).ToList();

                var continentDay = new DayWithLocationsData
                {
                    DayNumber = countryDay.DayNumber,
                    Date = countryDay.Date,
                    LocationsData = continentsData.Where(continentData => !string.IsNullOrEmpty(continentData.Continent)).ToList()
                };

                continentTimeSeries.DaysWithLocationData.Add(continentDay);
            }

            this.AdjustRealDaysData(continentTimeSeries);

            return continentTimeSeries;
        }

        private PredictionTimeSeries CreatePredictionTimeSeries(TimeSeriesByDay timeSeriesByDay, Func<double, double> regressionFunction)
        {
            var firstDay = timeSeriesByDay.DaysWithLocationData.First();

            var predictionDaysData = new List<PredictionDayData>
            {
                new PredictionDayData
                {
                    DayNumber = firstDay.DayNumber,
                    Date = firstDay.Date,
                    TotalCases = firstDay.TotalCases,
                    NewCases = firstDay.NewCases
                }
            };

            while (predictionDaysData.Count < (timeSeriesByDay.DaysWithLocationData.Count + 192))
            {
                var lastPredictionDay = predictionDaysData.Last();

                var realDay = timeSeriesByDay.DaysWithLocationData.FirstOrDefault(day => day.DayNumber == (lastPredictionDay.DayNumber + 1));

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

        private void AdjustRealDaysData(TimeSeriesByDay timeSeriesByDay)
        {
            foreach (var day in timeSeriesByDay.DaysWithLocationData)
            {
                var previousDay = timeSeriesByDay.DaysWithLocationData.GetPreviousElement(day, 1);

                day.NewCases = previousDay == null ? 0 : day.TotalCases - previousDay.TotalCases;

                var previousWeekDay = timeSeriesByDay.DaysWithLocationData.GetPreviousElement(day, 7);
                day.WeeklyNewCases = previousWeekDay == null ? 0 : (day.TotalCases - previousWeekDay.TotalCases) / 7;

                foreach (var locationData in day.LocationsData)
                {
                    locationData.NewCases = previousDay == null ? 0 : locationData.TotalCases - previousDay.LocationsData.Single(previousDayLocation => previousDayLocation.Equals(locationData)).TotalCases;
                    locationData.WeeklyNewCases = previousWeekDay == null ? 0 : locationData.TotalCases - previousWeekDay.LocationsData.Single(previousWeekDayLocation => previousWeekDayLocation.Equals(locationData)).TotalCases;
                }
            }
        }
    }
}