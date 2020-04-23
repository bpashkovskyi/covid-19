namespace Covid19.Models.Entities
{
    using System;

    public class DayData
    {
        public long DayNumber { get; set; }
        public DateTime Date { get; set; }

        public long TotalCases { get; set; }
        public long NewCases { get; set; }
        public double IncreaseRate { get; set; }

        public PredictionDayData ToPredictionDayData()
        {
            return new PredictionDayData
            {
                DayNumber = this.DayNumber,
                Date = this.Date,
                TotalCases = this.TotalCases,
                NewCases = this.NewCases,
                IncreaseRate = this.IncreaseRate,
                PredictionTotalCases = this.TotalCases,
                PredictionNewCases = this.NewCases,
                PredictionIncreaseRate = this.IncreaseRate,
            };
        }
    }
}
