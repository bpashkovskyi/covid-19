namespace Covid19.Models.Entities
{
    using System.Collections.Generic;

    public class TimeSeriesByLocation
    {
        public List<LocationWithDaysData> LocationsWithDayData { get; set; } = new List<LocationWithDaysData>();
    }
}