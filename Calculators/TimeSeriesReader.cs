namespace Covid19.Calculators
{
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;

    using ApplicationModels;
    using DomainModels;

    public class TimeSeriesReader
    {
        private const string ConfirmedCsvUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_confirmed_global.csv";
        private const string DeathCsvUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_deaths_global.csv";
        private const string RecoveredCsvUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_recovered_global.csv";

        public TimeSeriesReadOutputModel ReadTimeSeries(TimeSeriesReadInputModel timeSeriesReadInputModel)
        {
            var timeSeriesUrl = GetTimeSeriesUrl(timeSeriesReadInputModel.TimeSeriesType);
            var streamReader = GetStreamReaderForRemoteUrl(timeSeriesUrl);

            var timeSeriesDataTable = ConvertCsvToDataTable(streamReader);

            var timeSeries = ReadTimeSeries(timeSeriesDataTable, timeSeriesReadInputModel.Countries, timeSeriesReadInputModel.CountrySearchType);

            return new TimeSeriesReadOutputModel { TimeSeries = timeSeries };
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

        private TimeSeries ReadTimeSeries(DataTable timeSeriesDataTable, List<string> countries, CountrySearchType countrySearchType)
        {
            var dayNumber = 1;
            var daysData = new List<DayData>();

            for (var columnIndex = 4; columnIndex < timeSeriesDataTable.Columns.Count; columnIndex++)
            {
                var dayData = new DayData
                {
                    DayNumber = dayNumber,
                    DateString = timeSeriesDataTable.Columns[columnIndex].ColumnName,
                    TotalCases = 0,
                };

                for (var rowIndex = 0; rowIndex < timeSeriesDataTable.Rows.Count; rowIndex++)
                {
                    var currentCountryName = timeSeriesDataTable.Rows[rowIndex][1].ToString();
                    var dailyCases = int.Parse(timeSeriesDataTable.Rows[rowIndex][columnIndex].ToString());

                    switch (countrySearchType)
                    {
                        case CountrySearchType.Inside when countries.Contains(currentCountryName):
                        case CountrySearchType.Outside when !countries.Contains(currentCountryName):
                            dayData.TotalCases += dailyCases;
                            break;
                    }
                }
                dayNumber++;
                daysData.Add(dayData);
            }

            return new TimeSeries { DaysData = daysData };
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