namespace PortfolioBI.Models
{
    public class CombinedViewModel
    {
        public StockSummary GLWSummary { get; set; }
        public StockSummary NVDASummary { get; set; }
        public FinancialData GLWSpike { get; set; }
        public FinancialData NVDASpike { get; set; }
    }

    public class StockSummary
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double Average { get; set; }
    }
}
