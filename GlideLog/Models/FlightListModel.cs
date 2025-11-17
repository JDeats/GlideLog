using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CsvHelper;
using GlideLog.Data;
using GlideLog.DirectoryManagement;
using System.Globalization;

namespace GlideLog.Models
{
	public class FlightListModel
	{
		private readonly FlightDatabase _database;
		private readonly CancellationTokenSource cts;

        public FlightListModel(FlightDatabase database)
        {
			_database = database;
			cts = new();
		}

		public async Task<bool> ClearFlightsFromDatabase()
		{
			return await _database.ClearFlights();
		}

		public async Task<List<FlightEntryModel>> GetFlightsFromDataBase()
		{
			return await _database.GetFlightsAsync();
		}

		public async Task<bool> DeleteFlightFromDatabaseAsync(FlightEntryModel flightEntryModel)
		{
			if(await _database.DeleteFlightAsync(flightEntryModel) == 1)
			{
				return true;
			}
			return false;
		}


        public async Task<bool> ExportFromDatabaseAsync()
		{
			// Get the flights from the database
			List<FlightEntryModel> dbFlights = await _database.GetFlightsAsync();

			// Write to csv file
			if(dbFlights.Count > 0)
			{
				try
				{
					string fileName = "GlideLog.csv";
					string fullPath = string.Empty;

					// Android save location
					if (DeviceInfo.Current.Platform == DevicePlatform.Android)
					{
						string androidInternalDocuments = FilePathHelper.GetInternalDocumentsPath();    // "/storage/emulated/0/Documents";
						fullPath = Path.Combine(androidInternalDocuments, fileName);
					}

					// iOS save location
					else if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
					{
						fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
					}

					else if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
					{
						var folderPickerResult = await FolderPicker.PickAsync(cts.Token);
						if (folderPickerResult.IsSuccessful)
						{
                            fullPath = Path.Combine(folderPickerResult.Folder.Path, fileName);
                        }
					}

					if(string.IsNullOrEmpty(fullPath))
					{
						throw new DirectoryNotFoundException("Failed to get the save file location");
					}

                    //string targetFile = "GlideLog.csv"; // System.IO.Path.Combine(path, "GlideLog.csv");
                    //string file = Path.Combine(path, targetFile);  // FileSystem.AppDataDirectory  FileSystem.CacheDirectory

                    //		var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
                    //		csv.WriteHeader<CsvFlightEntry>();
                    //		csv.NextRecord();
                    //		csv.WriteRecords(strippedID);
                    //		//SaveFile(cts.Token, targetFile, memoryStream);

                    //		//File.WriteAllBytes(file, memoryStream.ToArray());

                    //		//await Share.Default.RequestAsync(new ShareFileRequest
                    //		//{
                    //		//	Title = targetFile,
                    //		//	File = new ShareFile(file)
                    //		//});

                    var statusRead = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                    var statusWrite = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

                    if (statusRead == PermissionStatus.Granted && statusWrite == PermissionStatus.Granted)
					{
						using (StreamWriter writer = new StreamWriter(fullPath))
						{
							// header
							writer.WriteLine($"{nameof(FlightEntryModel.DateTime)},{nameof(FlightEntryModel.Site)},{nameof(FlightEntryModel.Glider)},{nameof(FlightEntryModel.FlightCount)}," +
											 $"{nameof(FlightEntryModel.Hours)},{nameof(FlightEntryModel.Minutes)},{nameof(FlightEntryModel.OmitFromTotals)},{nameof(FlightEntryModel.Notes)}");

							// write lines
							foreach(FlightEntryModel flight in dbFlights)
							{
								// for the notes, replace right single quotation mark with apostrophe (excel can't translate replace right single quotation mark easily)
								char rightSingleQuotationMark = '’';
								string notes = flight.Notes;
								if (notes.Contains(rightSingleQuotationMark))
								{
									notes = notes.Replace(rightSingleQuotationMark, '\'');
								}

								// if the notes contain a comma then surround the text in double quotes
								if (notes.Contains(','))
								{
									notes = $"\"{notes}\"";
								}

								writer.WriteLine($"{flight.DateTime.ToString("M/d/yyyy H:mm")},{flight.Site},{flight.Glider},{flight.FlightCount},{flight.Hours},{flight.Minutes},{flight.OmitFromTotals},{notes}");
							}
						}
						return true;
					}
				}
                catch (Exception ex)
				{
					// TODO: handle exception properly
					string message = ex.Message;	
				}
			}
			return false;
		}

		public async Task<List<FlightEntryModel>> ImportToDatabaseAsync(string path)
		{
			List<CsvFlightEntry> flights = new List<CsvFlightEntry>();
			List<FlightEntryModel> flightsToReturn = new List<FlightEntryModel>();

			using (StreamReader reader = new StreamReader(path))
			{
				using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
				{
					// Get the flights from the csv file
					flights = csv.GetRecords<CsvFlightEntry>().ToList();

					// Get the flights from the database
					List<FlightEntryModel> dbFlights = await _database.GetFlightsAsync();

					// If the csv flight doesn't exist in the database then add it to the database and to the list of flights to be returned
					foreach(CsvFlightEntry flight in flights)
					{
						if (!LookForMatchingFlights(flight, dbFlights))
						{
							FlightEntryModel flightEntryModel = FlightEntryToFlightEntryModel(flight);
							if (await _database.SaveFlightAsync(flightEntryModel) == 1) //returns amount of rows added
							{
								flightsToReturn.Add(flightEntryModel);
							}
						}
					}
				}

			}
			
			return flightsToReturn;
		}

		private bool LookForMatchingFlights(CsvFlightEntry flightEntry, List<FlightEntryModel> flightEntryModels)
		{
			foreach (var model in flightEntryModels)
			{
				if (flightEntry.DateTime != model.DateTime) continue;
				if (!flightEntry.Site.Equals(model.Site)) continue;
				if (!flightEntry.Glider.Equals(model.Glider)) continue;
				if (flightEntry.FlightCount != model.FlightCount) continue;
				if (flightEntry.Hours != model.Hours) continue;
				if (flightEntry.Minutes != model.Minutes) continue;
				if (flightEntry.OmitFromTotals != model.OmitFromTotals) continue;
				if (!flightEntry.Notes.Equals(model.Notes)) continue;
				return true;
			}
			return false;
		}

		private FlightEntryModel FlightEntryToFlightEntryModel(CsvFlightEntry flightEntry)
		{
			return new FlightEntryModel()
			{
				DateTime = flightEntry.DateTime,
				Site = flightEntry.Site,
				Glider = flightEntry.Glider,
				FlightCount = flightEntry.FlightCount,
				Hours = flightEntry.Hours,
				Minutes = flightEntry.Minutes,
				OmitFromTotals = flightEntry.OmitFromTotals,
				Notes = flightEntry.Notes,
			};
		}

		private async Task SaveFile(CancellationToken cancellationToken, string name, MemoryStream stream)
		{
			var fileSaverResult = await FileSaver.Default.SaveAsync(name, stream, cancellationToken);
			if (fileSaverResult.IsSuccessful)
			{
				await Toast.Make($"The file was saved successfully to location: {fileSaverResult.FilePath}").Show(cancellationToken);
			}
			else
			{
				await Toast.Make($"The file was not saved successfully with error: {fileSaverResult.Exception.Message}").Show(cancellationToken);
			}
		}
	}
}
