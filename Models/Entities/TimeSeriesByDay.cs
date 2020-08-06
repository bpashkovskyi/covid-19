namespace Covid19.Models.Entities
{
    using System.Collections.Generic;
    using System.Linq;

    public class TimeSeriesByDay
    {
        public List<DayWithLocationsData> DaysWithLocationData { get; set; } = new List<DayWithLocationsData>();

        public TimeSeriesByLocation ToTimeSeriesByLocation()
        {
            var timeSeriesByLocation = new TimeSeriesByLocation();

            foreach (var firstDayDataLocation in this.DaysWithLocationData.First().LocationsData)
            {
                var locationData = new LocationWithDaysData
                {
                    Country = firstDayDataLocation.Country,
                    Continent = firstDayDataLocation.Continent
                };

                foreach (var dayData in this.DaysWithLocationData)
                {
                    var locationDayData = dayData.LocationsData.Single(currentLocationData => locationData.Equals(currentLocationData));

                    var dayLocationData = new DayData
                    {
                        Date = dayData.Date,
                        DayNumber = dayData.DayNumber,
                        TotalCases = locationDayData.TotalCases,
                        NewCases = locationDayData.NewCases,
                        WeeklyNewCases = locationDayData.WeeklyNewCases
                    };

                    locationData.DayLocationData.Add(dayLocationData);
                }

                timeSeriesByLocation.LocationsWithDayData.Add(locationData);
            }

            return timeSeriesByLocation;
        }
    }
}