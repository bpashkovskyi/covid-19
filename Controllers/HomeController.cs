using Microsoft.AspNetCore.Mvc;

namespace Covid19.Controllers
{
    using Covid19.Calculators;

    public class HomeController : Controller
    {
        private PredictionCalculator predictionCalculator;

        public HomeController()
        {
            this.predictionCalculator = new PredictionCalculator();
        }

        public IActionResult Index()
        {
            var timeSeries = this.predictionCalculator.CreateTimeSeries();
            return this.View(timeSeries);
        }
    }
}
