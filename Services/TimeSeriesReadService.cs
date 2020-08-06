namespace Covid19.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;

    using Covid19.ApplicationModels;
    using Covid19.Models.Entities;
    using Covid19.Models.Enums;
    using Covid19.Utilities;

    public class TimeSeriesReadService
    {
        private const string ConfirmedCsvUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_confirmed_global.csv";
        private const string DeathCsvUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_deaths_global.csv";
        private const string RecoveredCsvUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_recovered_global.csv";

        private readonly CountryToContinentConverter countryToContinentConverter = new CountryToContinentConverter();

        public TimeSeriesReadOutputModel ReadTimeSeries(TimeSeriesReadInputModel timeSeriesReadInputModel)
        {
            var timeSeriesUrl = this.GetTimeSeriesUrl(timeSeriesReadInputModel.TimeSeriesType);
            var streamReader = this.GetStreamReaderForRemoteUrl(timeSeriesUrl);

            var timeSeriesDataTable = this.ConvertCsvToDataTable(streamReader);
            var timeSeries = this.ReadTimeSeries(timeSeriesDataTable);

            return new TimeSeriesReadOutputModel { TimeSeriesByDay = timeSeries };
        }

        private StreamReader GetStreamReaderForRemoteUrl(string url)
        {
            var webRequest = WebRequest.Create(url);

            var webResponse = webRequest.GetResponse();
            var responseStream = webResponse.GetResponseStream();
            var streamReader = new StreamReader(responseStream);

            return streamReader;
        }

        private DataTable ConvertCsvToDataTable(StreamReader streamReader)
        {
            var headers = streamReader.ReadLine().Split(',');
            var dataTable = new DataTable();

            foreach (var header in headers)
            {
                dataTable.Columns.Add(header);
            }

            while (!streamReader.EndOfStream)
            {
                var rows = Regex.Split(streamReader.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                var dataRow = dataTable.NewRow();

                for (var i = 0; i < headers.Length; i++)
                {
                    dataRow[i] = rows[i];
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        private TimeSeriesByDay ReadTimeSeries(DataTable timeSeriesDataTable)
        {
            var dayNumber = 1;
            var daysData = new List<DayWithLocationsData>();

            for (var columnIndex = 4; columnIndex < timeSeriesDataTable.Columns.Count; columnIndex++)
            {
                var dayData = new DayWithLocationsData
                {
                    DayNumber = dayNumber,
                    Date = DateTime.Parse(timeSeriesDataTable.Columns[columnIndex].ColumnName)
                };

                for (var rowIndex = 0; rowIndex < timeSeriesDataTable.Rows.Count; rowIndex++)
                {
                    var currentCountryName = timeSeriesDataTable.Rows[rowIndex][1].ToString();
                    var totalCases = int.Parse(timeSeriesDataTable.Rows[rowIndex][columnIndex].ToString());

                    var locationData = new LocationData
                    {
                        Country = currentCountryName,
                        Continent = this.countryToContinentConverter.GetContinentByCountry(currentCountryName),
                        TotalCases = totalCases
                    };

                    dayData.LocationsData.Add(locationData);
                }

                daysData.Add(dayData);
                dayNumber++;
            }

            return new TimeSeriesByDay { DaysWithLocationData = daysData };
        }

        private string GetTimeSeriesUrl(TimeSeriesType timeSeriesType)
        {
            switch (timeSeriesType)
            {
                case TimeSeriesType.Confirmed: return ConfirmedCsvUrl;
                case TimeSeriesType.Death: return DeathCsvUrl;
                case TimeSeriesType.Recovered: return RecoveredCsvUrl;
                default: return null;
            }
        }
    }
}