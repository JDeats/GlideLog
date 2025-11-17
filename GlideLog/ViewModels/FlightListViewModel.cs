using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GlideLog.Models;
using GlideLog.Views;
using System.Collections.ObjectModel;

namespace GlideLog.ViewModels
{
    public partial class FlightListViewModel : ObservableObject
    {
        private readonly FlightListModel _flightListModel;
		private readonly CancellationTokenSource _cancellationTokenSource = new();
		private bool _firstLoad = true;

		public int ScrollPosition { get; set; }

		public Action<int> ScrollToItemAction { get; set; }

		[ObservableProperty]
        public partial ObservableCollection<FlightEntryModel> Flights { get; set; }

        public FlightListViewModel(FlightListModel flightListModel)
        {
            Flights = [];
            _flightListModel = flightListModel;
		}

		[RelayCommand]
		async Task AddFlight()
        {
			await Shell.Current.GoToAsync(nameof(AddFlightEntryView));
		}

		public async Task FireTestToast()
		{
			await HandleToast("Test Toast Success");
		}

		public async Task ClearFlights()
		{
			if (await _flightListModel.ClearFlightsFromDatabase())
			{
				Flights.Clear();
				await HandleToast("Successfully cleared all flights");
			}
			else
			{
				await HandleToast("Failed to clear all flights");
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

		public async Task ExportAsync()
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
						await HandleToast("Successfully Exported the Glide Log");
					}
                    else
                    {
						await HandleToast("Failed To Export the Glide Log");
					}
				//}
			}
			catch (Exception ex)
			{
				await HandleToast($"Failed To Export the Glide Log: {ex.Message}");
			}
		}

		private async Task HandleToast(string message)
		{
			_cancellationTokenSource.TryReset();
			var toast = Toast.Make(message);
			await toast.Show(_cancellationTokenSource.Token);
		}

        public async Task ImportAsync()
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
				await HandleToast($"Failed To Import Glide Log: {ex.Message}");
			}
		}

		public async Task OnAppearingAsync()
		{
			try
			{
				if (_firstLoad)
				{
					await HandleToast("Loading Flights");
					_firstLoad = false;
				}
				List<FlightEntryModel> dbFlights = await _flightListModel.GetFlightsFromDataBase();
				UpdateFlightsCollection(dbFlights);
				if (!_firstLoad)
				{
					ScrollToItemAction.Invoke(ScrollPosition);
				}
			}
			catch (Exception ex)
			{
				await HandleToast($"Failed To Load Flights From the Database: {ex.Message}");
			}
		}

		public void UpdateFlightsCollection(List<FlightEntryModel> flightEntryModels)
        {
			List<FlightEntryModel> ordered = [.. flightEntryModels.OrderByDescending(x => x.DateTime)];
			Flights.Clear();
			foreach (FlightEntryModel flight in ordered)
			{
				Flights.Add(flight);
			}
		}
	}
}
