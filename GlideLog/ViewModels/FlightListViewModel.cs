using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GlideLog.Models;
using GlideLog.Views;
using System.Collections.ObjectModel;

namespace GlideLog.ViewModels
{
    public partial class FlightListViewModel : ObservableObject
    {
        private FlightListModel _flightListModel;
		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private bool _firstLoad = true;

		[ObservableProperty]
        public partial ObservableCollection<FlightEntryModel> Flights { get; set; }

        public FlightListViewModel(FlightListModel flightListModel)
        {
            Flights = new();
            _flightListModel = flightListModel;
		}

		[RelayCommand]
		async Task AddFlight()
        {
            await Shell.Current.GoToAsync(nameof(AddFlightEntryView));
        }

        public async Task OnAppearingAsync()
        {
            try
            {
				if (_firstLoad) 
				{
					string message = $"Loading Flights";
					var toast = Toast.Make(message);
					await toast.Show(_cancellationTokenSource.Token);
					_firstLoad = false;
				}
				List<FlightEntryModel> dbFlights = await _flightListModel.GetFlightsFromDataBase();
				UpdateFlightsCollection(dbFlights);
            }
            catch(Exception ex)
            {
				string message = $"Failed To Load Flights From the Database: {ex.Message}";
				var toast = Toast.Make(message);
				await toast.Show(_cancellationTokenSource.Token);
			}
        }

        [RelayCommand]
		async Task DeleteFlight(FlightEntryModel flightEntryModel)
		{
            if (Flights.Contains(flightEntryModel))
			{
                await _flightListModel.DeleteFlightFromDatabaseAsync(flightEntryModel);
                Flights.Remove(flightEntryModel);
            }
		}

		[RelayCommand]
		async Task EditFlight(FlightEntryModel flightEntryModel)
		{
			var navigationParameter = new ShellNavigationQueryParameters
	        {
		        { "Flight", flightEntryModel }
	        };

			await Shell.Current.GoToAsync(nameof(EditFlightEntryView), navigationParameter);
		}

		[RelayCommand]
		async Task Export()
		{
			try
			{
				//var folderPickerResult = await FolderPicker.PickAsync(_cancellationTokenSource.Token);
				//if (folderPickerResult.IsSuccessful)
				//{
					PermissionStatus statusread = await Permissions.RequestAsync<Permissions.StorageRead>();
                    PermissionStatus statuswrite = await Permissions.RequestAsync<Permissions.StorageWrite>();

                    if (await _flightListModel.ExportFromDatabaseAsync())
                    {
						string message = $"Successfully Exported the Glide Log";
						await Toast.Make(message).Show(_cancellationTokenSource.Token);
					}
                    else
                    {
						string message = $"Failed To Export the Glide Log";
						await Toast.Make(message).Show(_cancellationTokenSource.Token);
					}
				//}
			}
			catch (Exception ex)
			{
				string message = $"Failed To Export the Glide Log: {ex.Message}";
				var toast = Toast.Make(message);
				await toast.Show(_cancellationTokenSource.Token);
			}
		}

		[RelayCommand]
        async Task Import()
        {
            try
            {
                PickOptions pickOptions = new PickOptions() { PickerTitle = "Select the Glide Log" };
                FileResult? result = await FilePicker.Default.PickAsync();
                if (result != null)
                {
                    if(result.FileName.EndsWith("csv", StringComparison.OrdinalIgnoreCase))
                    {
                        List<FlightEntryModel> flightEntryModels = await _flightListModel.ImportToDatabaseAsync(result.FullPath);
                        if(flightEntryModels.Count > 0)
                        {
                            UpdateFlightsCollection(flightEntryModels);
						}
					}
                }
            }
            catch (Exception ex)
            {
				string message = $"Failed To Import Glide Log: {ex.Message}";
				var toast = Toast.Make(message);
				await toast.Show(_cancellationTokenSource.Token);
			}
		}

        public void UpdateFlightsCollection(List<FlightEntryModel> flightEntryModels)
        {
            List<FlightEntryModel> ordered = flightEntryModels.OrderByDescending(x => x.DateTime).ToList();
            Flights.Clear();
			foreach (FlightEntryModel flight in ordered)
			{
				Flights.Add(flight);
			}
		}
	}
}
