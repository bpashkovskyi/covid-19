namespace Covid19.DomainModels
{
    using System.Collections.Generic;

    using ApplicationModels;

    public class PredictedTimeSeries
    {
        public List<DayData> DaysData { get; set; }

        public LinearRegressionCoefficients RegressionCoefficients { get; set; }

        public int UseLastDays { get; set; }
        public TimeSeriesType TimeSeriesType { get; set; }
        public string Countries { get; set; }
        public CountrySearchType CountrySearchType { get; set; }
        public string ViewType { get; set; }
        public bool UseAverage { get; set; }
    }
}
