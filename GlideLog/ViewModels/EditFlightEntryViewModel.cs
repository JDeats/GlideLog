using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GlideLog.Models;
using System.Threading;

namespace GlideLog.ViewModels
{
	[QueryProperty("Flight", "Flight")]

	public partial class EditFlightEntryViewModel : ObservableObject
	{
		private EditFlightEntryModel _editFlightEntryModel;
		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		public EditFlightEntryViewModel(EditFlightEntryModel editFlightEntryModel)
        {
			_editFlightEntryModel = editFlightEntryModel;
		}

		[ObservableProperty]
		public partial FlightEntryModel Flight { get; set; }

		[ObservableProperty]
		public partial string Date { get; set; } = DateTime.Now.ToString("M/d/yyyy");

		[ObservableProperty]
		public partial TimeSpan Time { get; set; } = DateTime.Now.TimeOfDay;

		[ObservableProperty]
		public partial string Site { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string Glider { get; set; } = string.Empty;

		[ObservableProperty]
		public partial int FlightCount { get; set; } = 1;

		[ObservableProperty]
		public partial int Hours { get; set; }

		[ObservableProperty]
		public partial int Minutes { get; set; }

		[ObservableProperty]
		public partial bool OmitFromTotals { get; set; }

		[ObservableProperty]
		public partial string Notes { get; set; } = string.Empty;

		public async Task OnAppearingAsync()
		{
			try
			{
				Date = Flight.DateTime.ToString("M/d/yyyy");
				Time = Flight.DateTime.TimeOfDay;
				Site = Flight.Site;
				Glider = Flight.Glider;
				FlightCount = Flight.FlightCount;
				Hours = Flight.Hours;
				Minutes = Flight.Minutes;
				OmitFromTotals = Flight.OmitFromTotals;
				Notes = Flight.Notes;
			}
			catch (Exception ex)
			{
				string message = $"Failed To Load the Flight Selected: {ex.Message}";
				var toast = Toast.Make(message);
				await toast.Show(_cancellationTokenSource.Token);
			}
		}

		[RelayCommand]
		public async Task Delete()
		{
			try
			{
				if(await _editFlightEntryModel.DeleteFlightFromDatabaseAsync(Flight))
				{
					string message = $"Successfully Deleted the Flight";
					var toast = Toast.Make(message);
					await toast.Show(_cancellationTokenSource.Token);
				}
				else
				{
					string message = $"Failed to Delete the Flight";
					var toast = Toast.Make(message);
					await toast.Show(_cancellationTokenSource.Token);
				}
				await Shell.Current.GoToAsync("..");
			}
			catch (Exception ex)
			{
				string message = $"Failed to Delete the Flight: {ex.Message}";
				var toast = Toast.Make(message);
				await toast.Show(_cancellationTokenSource.Token);
				await Shell.Current.GoToAsync("..");
			}
		}

		[RelayCommand]
		public async Task Update()
		{
			try
			{
				string[] _dateTime = Date.Split(' ');
				if (DateTime.TryParse($"{_dateTime[0]} {Time}", out DateTime result))
				{
					FlightEntryModel flightModel = new FlightEntryModel()
					{
						ID = Flight.ID,
						DateTime = result,
						Site = this.Site,
						Glider = this.Glider,
						FlightCount = this.FlightCount,
						Hours = this.Hours,
						Minutes = this.Minutes,
						OmitFromTotals = this.OmitFromTotals,
						Notes = this.Notes
					};
					if(await _editFlightEntryModel.UpdateDatabaseAsync(flightModel))
					{
						string message = $"Flight Successfully Updated";
						var toast = Toast.Make(message);
						await toast.Show(_cancellationTokenSource.Token);
					}
					else
					{
						string message = $"Failed to Update the Flight";
						var toast = Toast.Make(message);
						await toast.Show(_cancellationTokenSource.Token);
					}
				}
				else
				{
					string message = $"Failed to Parse the Date/Time";
					var toast = Toast.Make(message);
					await toast.Show(_cancellationTokenSource.Token);
				}
				await Shell.Current.GoToAsync("..");
			}
			catch (Exception ex)
			{
				string message = $"Failed to Update the Flight: {ex.Message}";
				var toast = Toast.Make(message);
				await toast.Show(_cancellationTokenSource.Token);
				await Shell.Current.GoToAsync("..");
			}
		}
    }
}
