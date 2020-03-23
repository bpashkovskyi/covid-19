namespace Covid19.Models
{
    using System;

    public class DailyData
    {
        public DateTime? Date { get; set; }
        public long DayNumber { get; set; }
        public string DateString { get; set; }
        
        public long? TotalCases { get; set; }
        public double? IncreaseRate { get; set; }

        public long PredictedTotalCasesFrom { get; set; }
        public long PredictedTotalCasesTo { get; set; }

        public long? NewCases { get; set; }
        public long PredictedNewCasesFrom { get; set; }
        public long PredictedNewCasesTo { get; set; }
    }
}
