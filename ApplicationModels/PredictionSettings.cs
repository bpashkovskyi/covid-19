namespace Covid19.ApplicationModels
{
    using Covid19.Models.Enums;

    public class PredictionSettings
    {
        public TimeSeriesType TimeSeriesType { get; set; }
        public string Countries { get; set; }
        public CountrySearchType CountrySearchType { get; set; }
        public string ViewType { get; set; }
    }
}