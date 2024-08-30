using SQLite;

namespace GlideLog.Models
{
	public class FlightEntryModel
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }
		public DateTime DateTime { get; set; }
		public string Site {  get; set; } = string.Empty;
		public string Glider { get; set; } = string.Empty;
		public int FlightCount { get; set; }
		public int Hours { get; set; }
		public int Minutes { get; set; }
		public bool OmitFromTotals { get; set; }
		public string Notes { get; set; } = string.Empty;

		public override bool Equals(object? obj)
		{
			if (obj == null) return false;

			FlightEntryModel? flightEntryModel = obj as FlightEntryModel;

			if (flightEntryModel == null) return false;

			if(flightEntryModel.ID != ID) return false;
			if(flightEntryModel.DateTime != DateTime) return false;
			if(flightEntryModel.Site != Site) return false;
			if(flightEntryModel.Notes != Notes) return false;
			if(flightEntryModel.Glider != Glider) return false;
			if(flightEntryModel.Hours != Hours) return false;
			if (flightEntryModel.Minutes != Minutes) return false;
			if(FlightCount != FlightCount) return false;
			if(OmitFromTotals != OmitFromTotals) return false;

			return true;
		}
	}

	public class CsvFlightEntry
	{
		public DateTime DateTime { get; set; }
		public string Site { get; set; } = string.Empty;
		public string Glider { get; set; } = string.Empty;
		public int FlightCount { get; set; }
		public int Hours { get; set; }
		public int Minutes { get; set; }
		public bool OmitFromTotals { get; set; }
		public string Notes { get; set; } = string.Empty;
	}
}
