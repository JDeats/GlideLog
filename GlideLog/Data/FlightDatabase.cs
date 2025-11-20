using CommunityToolkit.Maui.Alerts;
using GlideLog.Models;
using SQLite;

namespace GlideLog.Data
{
	public class FlightDatabase
	{
		SQLiteAsyncConnection? Database;

        public FlightDatabase()
        {

        }

		public async Task<bool> ClearFlights()
		{
			bool success = false;
			if (Database is not null)
			{
				try
				{
					await Database.DropTableAsync<FlightEntryModel>();
					CreateTableResult result = await Database.CreateTableAsync<FlightEntryModel>();
					if (result == CreateTableResult.Created || result == CreateTableResult.Migrated)
					{
						success = true;
					}
					else
					{
						var toast = Toast.Make(result.ToString());
						await toast.Show();
						await Task.Delay(3000);
					}
				}
				catch (Exception ex)
				{
					var toast = Toast.Make(ex.Message);
					await toast.Show();
					await Task.Delay(3000);
				}
			}
			return success;
		}

		async Task Init()
		{
			if (Database is not null)
				return;

			Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
			await Database.CreateTableAsync<FlightEntryModel>();
		}

		public async Task<List<FlightEntryModel>> GetFlightsAsync()
		{
			await Init();
			return await Database!.Table<FlightEntryModel>().ToListAsync();
		}

		public async Task<FlightEntryModel> GetFlightAsync(int id)
		{
			await Init();
			return await Database!.Table<FlightEntryModel>().Where(i => i.ID == id).FirstOrDefaultAsync();
		}

		public async Task<int> SaveFlightAsync(FlightEntryModel flightEntry)
		{
			await Init();
			if (flightEntry.ID != 0)
			{
				return await Database!.UpdateAsync(flightEntry);
			}
			else
			{
				return await Database!.InsertAsync(flightEntry);
			}
		}

		public async Task<int> DeleteFlightAsync(FlightEntryModel flightEntry)
		{
			await Init();
			return await Database!.DeleteAsync(flightEntry);
		}
	}
}
