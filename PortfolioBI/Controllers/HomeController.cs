using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortfolioBI.Data;
using PortfolioBI.Models;
using System.Diagnostics;

namespace PortfolioBI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            DateTime startDate = new DateTime(2015, 1, 1);
            DateTime endDate = new DateTime(2015, 6, 30);

            var glwData = _context.FinancialData
                .Where(d => d.Ticker == "GLW" && d.Date >= startDate && d.Date <= endDate);
            var nvdaData = _context.FinancialData
                .Where(d => d.Ticker == "NVDA" && d.Date >= startDate && d.Date <= endDate);

            var stockSummary = new
            {
                GLWMin = glwData.Min(d => d.Close),
                GLWMax = glwData.Max(d => d.Close),
                GLWAvg = glwData.Average(d => d.Close),
                NVDAMin = nvdaData.Min(d => d.Close),
                NVDAMax = nvdaData.Max(d => d.Close),
                NVDAAvg = nvdaData.Average(d => d.Close)
            };

            string stockSummaryJson = JsonConvert.SerializeObject(stockSummary);

            return View(model: stockSummaryJson); // Pass the serialized JSON as a string
        }

        /// <summary>
        /// this method is used for uploading a CVS file with stock price data & inserting the data into our database table called FinancialData
        /// for times sake I'm just uploading the data that is generated from Yahoo Finance and not coming up with a method to handle it including the ticker
        /// i'm going to just manually insert the ticker price on the FinancialData model below 
        /// data source: https://finance.yahoo.com/quote/NVDA/history?period1=1420070400&period2=1435622400
        [HttpPost]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                if (file == null || file.Length == 0)
                    return Content("No file uploaded");

                if (!file.FileName.EndsWith(".csv"))
                    return Content("Invalid file format");

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    // skip the header line because it will cause the method to crash. For example first header is which is just a string value that will cause
                    //  date = DateTime.Parse(values[0]), // Assuming values[0] is the date string to throw an error
                    reader.ReadLine(); // Read and discard the first line (header)

                    string line;
                    // iterate over each line in the CVS file and insert it. Since total records inserted for our 
                    // example are under 1000 this will be fine. For larger data sets I would refactor this to use a BulkInsert
                    while ((line = reader.ReadLine()) != null)
                    {
                        var values = line.Split(',');
                        if (values.Length != 7)
                            continue; // Skip invalid lines

                        var financialData = new FinancialData
                        {
                            Date = DateTime.Parse(values[0]), // Assuming values[0] is the date string
                            Open = double.Parse(values[1]),
                            High = double.Parse(values[2]),
                            Low = double.Parse(values[3]),
                            Close = double.Parse(values[4]),
                            AdjustClose = double.Parse(values[5]),
                            Volume = int.Parse(values[6]),
                            Ticker = "NVDA" // I hard coded this per data set because yahoo doesn't include the ticker on the CVS
                        };

                        _context.FinancialData.Add(financialData);
                    }

                    _context.SaveChanges();
                }

                return RedirectToAction("Index", new { message = "Data uploaded successfully." });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", new { message = "Error uploading data: " + ex.Message });
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
