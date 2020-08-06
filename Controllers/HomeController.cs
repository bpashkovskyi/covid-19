namespace Covid19.Controllers
{
    using Covid19.ApplicationModels;
    using Covid19.Models.Enums;
    using Covid19.Services;

    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly PredictionService predictionService;

        public HomeController()
        {
            this.predictionService = new PredictionService();
        }

        public IActionResult Index(TimeSeriesType timeSeriesType = TimeSeriesType.Confirmed, string countries = "Ukraine", CountrySearchType countrySearchType = CountrySearchType.Inside, string viewType = "graph")
        {
            var predictionSettings = new PredictionSettings
            {
                TimeSeriesType = timeSeriesType,
                Countries = countries,
                CountrySearchType = countrySearchType,
                ViewType = viewType
            };

            var predictionOutput = this.predictionService.CreatePredictionTimeSeries(new PredictionInputModel { Settings = predictionSettings });

            return viewType == "table" ? this.View("Table", predictionOutput) : this.View("Graph", predictionOutput);
        }
    }
}