namespace Covid19.Models.Entities
{
    using System.Collections.Generic;

    public class TimeSeries
    {
        public List<DayData> DaysData { get; set; }
    }
}