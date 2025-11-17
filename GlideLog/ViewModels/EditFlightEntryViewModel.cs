using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GlideLog.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using static GlideLog.ViewModels.UserEntryPopupViewModel;

namespace GlideLog.ViewModels
{
	[QueryProperty("Flight", "Flight")]

	public partial class EditFlightEntryViewModel : ObservableObject
	{
		private readonly EditFlightEntryModel _editFlightEntryModel;
		private readonly CancellationTokenSource _cancellationTokenSource = new();
		private const string newSiteText = "Add New Site";
		private const string newGliderText = "Add New Glider";
		private readonly IPopupService _popupService;

		public EditFlightEntryViewModel(EditFlightEntryModel editFlightEntryModel, AddFlightEntryModel addFlightEntryModel, IPopupService popupService)
        {
			_editFlightEntryModel = editFlightEntryModel;

			Task.Factory.StartNew(async () =>
			{
				// populate sites
				IList<string> sites = await addFlightEntryModel.GetSites();
				foreach (string site in sites)
				{
					Sites.Add(site);
				}
				Sites.Insert(0, newSiteText);

				// populate gliders
				IList<string> gliders = await addFlightEntryModel.GetGliders();
				foreach (string glider in gliders)
				{
					Gliders.Add(glider);
				}
				Gliders.Insert(0, newGliderText);

			}, _cancellationTokenSource.Token);
			_popupService = popupService;
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

		private ObservableCollection<string> _gliders = [];

		public ObservableCollection<string> Gliders
		{
			get => _gliders;
			set => _gliders = value;
		}

		private ObservableCollection<string> _sites = [];

		public ObservableCollection<string> Sites
		{
			get => _sites;
			set => _sites = value;
		}

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

		public async Task DisplayPopup(EntryPopupType entryPopupType)
		{
			try
			{
				switch (entryPopupType)
				{
					case EntryPopupType.Site:
						var siteParameters = new Dictionary<string, object>
						{
							[nameof(UserEntryPopupViewModel.EntryLabel)] = "Site:"
						};
						IPopupResult<string>? siteResult = (IPopupResult<string>?)await _popupService.ShowPopupAsync<UserEntryPopupViewModel>(Shell.Current, options: null, siteParameters);
						if (siteResult != null && !string.IsNullOrEmpty(siteResult.Result))
						{
							Sites.Add(siteResult.Result);
							Site = siteResult.Result;
						}
						break;

					case EntryPopupType.Glider:
						var gliderParameters = new Dictionary<string, object>
						{
							[nameof(UserEntryPopupViewModel.EntryLabel)] = "Glider:"
						};
						IPopupResult<string>? gliderResult = (IPopupResult<string>?)await _popupService.ShowPopupAsync<UserEntryPopupViewModel>(Shell.Current, options: null, gliderParameters);
						if (gliderResult != null && !string.IsNullOrEmpty(gliderResult.Result))
						{
							Gliders.Add(gliderResult.Result);
							Glider = gliderResult.Result;
						}
						break;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		public async Task GliderPickerClosed()
		{
			if (Glider.Equals(newGliderText))
			{
				await DisplayPopup(EntryPopupType.Glider);
			}
		}

		public async Task SitePickerClosed()
		{
			if (Site.Equals(newSiteText))
			{
				await DisplayPopup(EntryPopupType.Site);
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
