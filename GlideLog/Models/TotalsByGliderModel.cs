namespace GlideLog.Models
{
	public class TotalsByGliderModel
	{
		public string Glider { get; set; } = string.Empty;
		public int TotalFlights { get; set; }
		public int TotalHours { get; set; }
		public int TotalMinutes { get; set; }
	}
}
