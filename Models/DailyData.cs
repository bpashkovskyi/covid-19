namespace Covid19.Models
{
    using System;

    public class DailyData
    {
        public DateTime Date { get; set; }
        public long DayNumber { get; set; }
        
        public long? TotalCases { get; set; }
        public long? NewCases { get; set; }
        public double? IncreaseRate { get; set; }
        public long PredictionFrom { get; set; }
        public long PredictionTo { get; set; }
    }
}
