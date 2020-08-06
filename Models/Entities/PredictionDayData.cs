namespace Covid19.Models.Entities
{
    using System;

    public class PredictionDayData
    {
        public long DayNumber { get; set; }
        public DateTime Date { get; set; }

        public long? TotalCases { get; set; }
        public long PredictionTotalCases { get; set; }

        public long? NewCases { get; set; }
        public long PredictionNewCases { get; set; }
        public long? WeeklyNewCases { get; set; }

        public long? DeathRate { get; set; }
        public long PredictionDeathRate { get; set; }
    }
}