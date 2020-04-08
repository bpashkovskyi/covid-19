namespace Covid19.DomainModels
{
    public class DayData
    {
        public long DayNumber { get; set; }
        public string DateString { get; set; }
        
        public long? TotalCases { get; set; }
        public long PredictedTotalCases { get; set; }

        public long? NewCases { get; set; }
        public long PredictedNewCases { get; set; }

        public double? IncreaseRate { get; set; }
        public double? PredictedIncreaseRate { get; set; }
    }
}
