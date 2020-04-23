﻿namespace Covid19.ApplicationModels
{
    using System.Collections.Generic;

    using Covid19.Models.Enums;

    public class TimeSeriesReadInputModel
    {
        public TimeSeriesType TimeSeriesType { get; set; }
        public List<string> Countries { get; set; }
        public CountrySearchType CountrySearchType { get; set; }
    }
}