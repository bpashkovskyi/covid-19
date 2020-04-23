using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Covid19.Controllers
{
    using ApplicationModels;

    using Covid19.Models.Enums;
    using Covid19.Services;

    public class HomeController : Controller
    {
        private readonly PredictionService predictionService;

        public HomeController()
        {
            this.predictionService = new PredictionService();
        }

        public IActionResult Index(int useLastDays = 30, TimeSeriesType timeSeriesType = TimeSeriesType.Confirmed, string countries = "China,US", CountrySearchType countrySearchType = CountrySearchType.Outside, string viewType = "table")
        {
            var predictionSettings = new PredictionSettings
            {
                UseLastDays = useLastDays,
                TimeSeriesType = timeSeriesType,
                Countries = countries,
                CountrySearchType = countrySearchType,
                ViewType = viewType,
            };
            var predictionOutput = this.predictionService.CreatePredictionTimeSeries(new PredictionInputModel { Settings = predictionSettings });

            return viewType == "table" ? View("Table", predictionOutput) : View("Graph", predictionOutput);
        }
    }
}
