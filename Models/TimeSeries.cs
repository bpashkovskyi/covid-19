using System.Collections.Generic;

namespace Covid19.Models
{
    using System;

    public class TimeSeries
    {
        public List<DailyData> DaysData { get; set; }

        public ExponentialRegressionCoefficients ExponentialRegressionCoefficients { get; set; }

        public DateTime BasedOnDate { get; set; }
    }
}
