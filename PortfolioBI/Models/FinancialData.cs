using System.ComponentModel.DataAnnotations;

namespace PortfolioBI.Models
{
    public class FinancialData
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double AdjustClose { get; set; }
        public int Volume { get; set; }
        public string Ticker { get; set; }
    }
}
