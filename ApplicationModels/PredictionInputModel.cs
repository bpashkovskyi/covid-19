namespace Covid19.ApplicationModels
{
    using System.Collections.Generic;

    public class PredictionInputModel
    {
        public int UseLastDays { get; set; }
        public TimeSeriesType TimeSeriesType { get; set; }
        public List<string> Countries { get; set; }
        public CountrySearchType CountrySearchType { get; set; }
        public string ViewType { get; set; }
        public bool UseAverage { get; set; }
    }
}
