namespace Covid19.Calculators
{
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;

    using Covid19.Models;

    public class TimeSeriesDataReader
    {
        public List<DailyData> GetDailyCasesOutsideChina(string timeSeriesFileUrl)
        {
            var streamReader = this.GetStreamReaderForRemoteUrl(timeSeriesFileUrl);
            var timeSeriesDataTable = this.ConvertCsvToDataTable(streamReader);

            var realCasesStatisticsByDayOutsideChina = this.GetDailyCasesOutsideChina(timeSeriesDataTable);

            return realCasesStatisticsByDayOutsideChina.ToList();
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

        private IEnumerable<DailyData> GetDailyCasesOutsideChina(DataTable timeSeriesDataTable)
        {
            var dayNumber = 1;
            for (var columnIndex = 4; columnIndex < timeSeriesDataTable.Columns.Count; columnIndex++)
            {
                var dailyData = new DailyData
                {
                    DayNumber = dayNumber,
                    TotalCases = 0
                };
                for (var rowIndex = 0; rowIndex < timeSeriesDataTable.Rows.Count; rowIndex++)
                {
                    var countryName = timeSeriesDataTable.Rows[rowIndex][1].ToString();
                    var dailyCases = int.Parse(timeSeriesDataTable.Rows[rowIndex][columnIndex].ToString());

                    if (countryName != "China")
                    {
                        dailyData.TotalCases += dailyCases;
                    }
                }
                dayNumber++;

                yield return dailyData;
            }
        }
    }
}