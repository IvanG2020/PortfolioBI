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

        // come back tomorrow and refactor and add more comments
        public async Task<IActionResult> Index()
        {
            DateTime startDate = new DateTime(2015, 1, 1);
            DateTime endDate = new DateTime(2015, 6, 30);

            var glwData = await _context.FinancialData
                .Where(d => d.Ticker == "GLW" && d.Date >= startDate && d.Date <= endDate)
                .Select(d => new { d.Date, d.Close })
                .OrderBy(d => d.Date)
                .ToListAsync();

            var nvdaData = await _context.FinancialData
                .Where(d => d.Ticker == "NVDA" && d.Date >= startDate && d.Date <= endDate)
                .Select(d => new { d.Date, d.Close })
                .OrderBy(d => d.Date)
                .ToListAsync();

            //prevent the site from crashing when a fresh copy is cloned because initially the database will be empty 
            if (glwData.Count == 0 || nvdaData.Count == 0)
                return View(model: null);


            var glwFinancialData = await _context.FinancialData
                .Where(d => d.Ticker == "GLW" && d.Date >= startDate && d.Date <= endDate)
                .OrderBy(d => d.Date)
                .ToListAsync();

            
            var nvdaFinancialData = await _context.FinancialData
                .Where(d => d.Ticker == "NVDA" && d.Date >= startDate && d.Date <= endDate)
                .OrderBy(d => d.Date)
                .ToListAsync();

            // first major glw spike
            var glwSpike = FindSignificantSpike(glwFinancialData);

            // first major nvda spike
            var nvdaSpike = FindSignificantSpike(nvdaFinancialData);

            // calcluate $1000 investment into GLW on 1/2/2015 and sell the first spike
            // 1/1/2015 was a sunday so market was closed
            var glwInvestment = glwData.FirstOrDefault();
            var glwROI = (1000 / glwInvestment?.Close) * glwSpike.Close;

            // Fetch the previous close price for GLW spike
            var glwPreviousPrice = glwData.FirstOrDefault(d => d.Date < glwSpike.Date)?.Close;

            // Fetch the previous close price for NVDA spike
            var nvdaPreviousPrice = nvdaData.FirstOrDefault(d => d.Date < nvdaSpike.Date)?.Close;

            // data we're passing to the front end 
            var data = new
            {
                GLWSummary = new StockSummary
                {
                    Min = glwData.Min(d => d.Close),
                    Max = glwData.Max(d => d.Close),
                    Average = glwData.Average(d => d.Close)
                },
                NVDASummary = new StockSummary
                {
                    Min = nvdaData.Min(d => d.Close),
                    Max = nvdaData.Max(d => d.Close),
                    Average = nvdaData.Average(d => d.Close)
                },
                GLWDates = glwData.Select(g => g.Date.ToShortDateString()),
                GLWPrices = glwData.Select(g => g.Close),
                NVDADates = nvdaData.Select(n => n.Date.ToShortDateString()),
                NVDAPrices = nvdaData.Select(n => n.Close),
                totalROI = glwROI,
                GLWSpike = new
                {
                    Date = glwSpike.Date,
                    Close = glwSpike.Close,
                    PreviousClose = glwPreviousPrice
                },
                NVDASpike = new
                {
                    Date = nvdaSpike.Date,
                    Close = nvdaSpike.Close,
                    PreviousClose = nvdaPreviousPrice
                }
            };

            string jsonData = JsonConvert.SerializeObject(data);
            return View(model: jsonData);
        }

        //come back to this later
        //private List<FinancialData> GetSpikes(List<FinancialData> data)
        //{
        //    var spikes = new List<FinancialData>();
        //    for (int i = 1; i < data.Count; i++)
        //    {
        //        double previousPrice = data[i - 1].Close;
        //        double currentPrice = data[i].Close;
        //        if (((currentPrice - previousPrice) / previousPrice) * 100 > 7) // 5% spike threshold
        //        {
        //            spikes.Add(data[i]);
        //        }
        //    }
        //    return spikes;
        //}


        // method used for finding significant price spike
        private FinancialData FindSignificantSpike(List<FinancialData> data)
        {
            FinancialData spike = null;
            double maxIncrease = 0;

            for (int i = 1; i < data.Count; i++)
            {
                double priceChange = data[i].Close - data[i - 1].Close;
                if (priceChange > maxIncrease)
                {
                    maxIncrease = priceChange;
                    spike = data[i];
                }
            }

            return spike;
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

        // auto generated code disregard
        public IActionResult Privacy()
        {
            return View();
        }

        // auto generated code disregard
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
