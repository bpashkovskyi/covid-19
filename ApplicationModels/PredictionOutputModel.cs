namespace Covid19.ApplicationModels
{
    using Covid19.Models.Entities;

    public class PredictionOutputModel
    {
        public PredictionTimeSeries AggregatedTimeSeries { get; set; }

        public TimeSeriesByLocation ContinentTimeSeries { get; set; }

        public TimeSeriesByLocation GrowingCountriesTimeSeries { get; set; }

        public TimeSeriesByLocation DecreasingCountriesTimeSeries { get; set; }

        public TimeSeriesByLocation SelectedCountriesTimeSeries { get; set; }

        public PredictionSettings Settings { get; set; }
    }
}