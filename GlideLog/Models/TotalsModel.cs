using GlideLog.Data;

namespace GlideLog.Models
{
	public class TotalsModel
	{
		private FlightDatabase _database;
		private List<FlightEntryModel>? _flightEntries;

		public TotalsModel(FlightDatabase database)
        {
			_database = database;
		}

		public async Task GetFlightEntriesAsync()
		{
			_flightEntries = await _database.GetFlightsAsync();
		}

		public async Task<int> GetTotalFlightsAsync()
		{
			int flightCount = 0;
			await Task.Run(() => 
			{
				foreach (FlightEntryModel flight in _flightEntries!)
				{
					if (!flight.OmitFromTotals)
					{
						flightCount += flight.FlightCount;
					}
				}
			});
			
			return flightCount;
		}

		public async Task<Tuple<int,int>> GetTotalFlightsHoursAsync()
		{
			int hours = 0;
			int minutes = 0;
			await Task.Run(() =>
			{
				foreach (FlightEntryModel flight in _flightEntries!)
				{
					if (!flight.OmitFromTotals)
					{
						minutes += flight.Minutes;
						if(minutes > 59)
						{
							hours++;
							minutes -= 60;
						}
						hours += flight.Hours;
					}
				}
			});

			return new Tuple<int, int> ( hours, minutes );
		}

		public async Task<Dictionary<string, (int, int, int)>> GetTotalsByMonthAsync()
		{
			// (int, int, int) = (Hours, Minutes, FlightCount)
			Dictionary<string, (int, int, int)> dateTotals = [];

			await Task.Run(() =>
			{
				foreach (FlightEntryModel flight in _flightEntries!)
				{
					if (!flight.OmitFromTotals)
					{
						int flightCount = flight.FlightCount;
						int year = flight.DateTime.Year;
						int minutes = flight.Minutes;
						int hours = flight.Hours;
						string monthYear = $"{flight.DateTime:MMMM} {year}";
						if (dateTotals.ContainsKey(monthYear))
						{
							int subMins = minutes + dateTotals[monthYear].Item2;
							int subHours = hours + dateTotals[monthYear].Item1;
							if (subMins > 60)
							{
								subHours++;
								subMins -= 60;
							}
							dateTotals[monthYear] = (subHours, subMins, flightCount + dateTotals[monthYear].Item3);
						}
						else
						{
							dateTotals.Add(monthYear, (hours, minutes, flightCount));
						}
					}
				}
			});

			return dateTotals;
		}

		public async Task<Dictionary<string, (int, int, int)>> GetTotalsBySiteAsync()
		{
			// (int, int, int) = (Hours, Minutes, FlightCount)
			Dictionary<string, (int, int, int)> siteTotals = [];

			await Task.Run(() =>
			{
				foreach (FlightEntryModel flight in _flightEntries!)
				{
					if (!flight.OmitFromTotals)
					{
						int flightCount = flight.FlightCount;
						string site = flight.Site;
						int minutes = flight.Minutes;
						int hours = flight.Hours;
						
						if (siteTotals.TryGetValue(site, out (int, int, int) value))
						{
							int subMins = minutes + value.Item2;
							int subHours = hours + value.Item1;
							if (subMins > 60)
							{
								subHours++;
								subMins -= 60;
							}
							siteTotals[site] = (subHours, subMins, flightCount + value.Item3);
						}
						else
						{
							siteTotals.Add(site, (hours, minutes, flightCount));
						}
					}
				}
			});

			return siteTotals;
		}
	}
}
