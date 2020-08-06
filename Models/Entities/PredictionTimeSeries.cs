namespace Covid19.Models.Entities
{
    using System.Collections.Generic;

    public class PredictionTimeSeries
    {
        public List<PredictionDayData> DaysData { get; set; } = new List<PredictionDayData>();
    }
}