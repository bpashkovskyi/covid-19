namespace Covid19.ApplicationModels
{
    using Covid19.Models.Entities;

    public class PredictionOutputModel
    {
        public PredictionTimeSeries TimeSeries { get; set; }

        public PredictionSettings Settings { get; set; }
    }
}