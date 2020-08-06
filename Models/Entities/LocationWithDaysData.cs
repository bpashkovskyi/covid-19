namespace Covid19.Models.Entities
{
    using System.Collections.Generic;

    public class LocationWithDaysData
    {
        public string Country { get; set; }
        public string Continent { get; set; }

        public List<DayData> DayLocationData { get; set; } = new List<DayData>();

        public bool Equals(LocationData locationData)
        {
            if (!string.IsNullOrEmpty(this.Country) && !string.IsNullOrEmpty(locationData.Country))
            {
                return this.Country.Equals(locationData.Country);
            }

            if (!string.IsNullOrEmpty(this.Continent) && !string.IsNullOrEmpty(locationData.Continent))
            {
                return this.Continent.Equals(locationData.Continent);
            }

            return false;
        }
    }
}