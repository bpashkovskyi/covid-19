namespace Covid19.Models.Entities
{
    using System;
    using System.Collections.Generic;

    public class DayWithLocationsData
    {
        public long DayNumber { get; set; }
        public DateTime Date { get; set; }

        public long TotalCases { get; set; }

        public long NewCases { get; set; }
        public long WeeklyNewCases { get; set; }

        public List<LocationData> LocationsData { get; set; } = new List<LocationData>();
    }
}