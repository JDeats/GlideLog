using GlideLog.Data;
using System.Diagnostics;

namespace GlideLog.Models
{
	public class EditFlightEntryModel
	{
		private FlightDatabase _database;
		public EditFlightEntryModel(FlightDatabase flightDatabase)
        {
			_database = flightDatabase;
		}

		public async Task<bool> UpdateDatabaseAsync(FlightEntryModel _flight)
		{
			// this will update the entry assuming the entry id is not equal to 0 and return the number of rows updated
			if(await _database.SaveFlightAsync(_flight) == 1)
			{
				return true;
			}

			return false;
		}

		public async Task<bool> DeleteFlightFromDatabaseAsync(FlightEntryModel _flight)
		{
			// this will delete the entry and return the number of rows deleted
			if (await _database.DeleteFlightAsync(_flight) == 1)
			{
				return true;
			}

			return false;
		}
	}
}
