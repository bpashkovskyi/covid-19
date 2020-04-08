using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Covid19.Controllers
{
    using ApplicationModels;
    using Calculators;

    public class HomeController : Controller
    {
        private readonly PredictionCalculator _predictionCalculator;

        public HomeController()
        {
            _predictionCalculator = new PredictionCalculator();
        }

        public IActionResult Index(int useLastDays = 30, TimeSeriesType timeSeriesType = TimeSeriesType.Confirmed, string countries = "China,US", CountrySearchType countrySearchType = CountrySearchType.Outside, bool useAverage = false, string viewType = "table")
        {
            var predictionModel = new PredictionInputModel
            {
                UseLastDays = useLastDays,
                TimeSeriesType = timeSeriesType,
                Countries = countries.Split(',').ToList(),
                CountrySearchType = countrySearchType,
                UseAverage = useAverage,
                ViewType = viewType,
            };
            var predictionTimeSeries = _predictionCalculator.CreatePredictionTimeSeries(predictionModel).TimeSeries;

            return viewType == "table" ? View("Table", predictionTimeSeries) : View("Graph", predictionTimeSeries);
        }
    }
}
