namespace Covid19.Models.Entities
{
    public class LocationData
    {
        public string Country { get; set; }
        public string Continent { get; set; }

        public long TotalCases { get; set; }

        public long NewCases { get; set; }
        public long WeeklyNewCases { get; set; }

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