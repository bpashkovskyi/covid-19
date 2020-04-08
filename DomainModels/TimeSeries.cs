namespace Covid19.DomainModels
{
    using System.Collections.Generic;

    public class TimeSeries
    {
        public List<DayData> DaysData { get; set; }
    }
}