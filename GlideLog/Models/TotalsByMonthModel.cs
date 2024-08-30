namespace GlideLog.Models
{
    public class TotalsByMonthModel
    {
		public string Date { get; set; } = string.Empty;
		public int TotalFlights { get; set; }
		public int TotalHours { get; set; }
		public int TotalMinutes { get; set; }
	}
}
