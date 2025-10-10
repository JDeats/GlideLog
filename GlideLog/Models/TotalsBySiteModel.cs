namespace GlideLog.Models
{
	public class TotalsBySiteModel
	{
		public string Site { get; set; } = string.Empty;
		public int TotalFlights { get; set; }
		public int TotalHours { get; set; }
		public int TotalMinutes { get; set; }
	}
}
