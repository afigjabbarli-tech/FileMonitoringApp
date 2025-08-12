namespace FileMonitoring.Core.Models
{
    public class FinancialRecord
    {
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }
        public FinancialRecord(DateTime date, decimal open, decimal high, decimal low, decimal close, long volume)
        {
            Date = date;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }
    }
}
