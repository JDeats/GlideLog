using GlideLog.Data;
using System.Diagnostics;

namespace GlideLog.Models
{
	public class AddFlightEntryModel
	{
		private FlightDatabase _database;

        public AddFlightEntryModel(FlightDatabase flightDatabase)
        {
			_database = flightDatabase;
		}

		public async Task<bool> AddFlightEntry(FlightEntryModel flightEntryModel)
		{
			try
			{
				if(!(await _database.GetFlightsAsync()).Contains(flightEntryModel))
				{
					if (await _database.SaveFlightAsync(flightEntryModel) == 1) //returns amount of rows added
					{
						return true;
					}
				}
			}
			catch { }
			return false;
		}

		public async Task<IList<string>> GetSites()
		{
			List<string> sites = new List<string>();
			try
			{
				List<FlightEntryModel> flights = await _database.GetFlightsAsync();
				foreach (FlightEntryModel flight in flights)
				{
					if (!sites.Contains(flight.Site))
					{
						sites.Add(flight.Site);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return sites;
		}

		public async Task<IList<string>> GetGliders()
		{
			List<string> gliders = new List<string>();
			try
			{
				List<FlightEntryModel> flights = await _database.GetFlightsAsync();
				foreach (FlightEntryModel flight in flights)
				{
					if (!gliders.Contains(flight.Glider))
					{
						gliders.Add(flight.Glider);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return gliders;
		}
	}
}
