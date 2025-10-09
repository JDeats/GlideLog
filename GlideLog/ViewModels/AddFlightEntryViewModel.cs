using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GlideLog.Models;
using GlideLog.Views;
using System.Collections.ObjectModel;
using static GlideLog.ViewModels.UserEntryPopupViewModel;

namespace GlideLog.ViewModels
{
	public partial class AddFlightEntryViewModel : ObservableObject
	{
        private AddFlightEntryModel _addFlightEntryModel;
		private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private const string newSiteText = "Add New Site";
		private const string newGliderText = "Add New Glider";
		private readonly IPopupService _popupService;

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

		private ObservableCollection<string> _gliders = new ObservableCollection<string>();

		public ObservableCollection<string> Gliders
		{
			get => _gliders;
			set => _gliders = value;
		}

		private ObservableCollection<string> _sites = new ObservableCollection<string>();

		public ObservableCollection<string> Sites
		{
			get => _sites;
			set => _sites = value;
		}

		public AddFlightEntryViewModel(AddFlightEntryModel addFlightEntryModel, IPopupService popupService)
        {
			_addFlightEntryModel = addFlightEntryModel;
            Task.Factory.StartNew(async () =>
            {
				// populate sites
				IList<string> sites = await _addFlightEntryModel.GetSites();
                foreach (string site in sites) 
                {
                    Sites.Add(site);
                }
				Sites.Insert(0, newSiteText);

				// populate gliders
				IList<string> gliders = await _addFlightEntryModel.GetGliders();
				foreach (string glider in gliders)
				{
					Gliders.Add(glider);
				}
				Gliders.Insert(0, newGliderText);

			}, tokenSource.Token);
			_popupService = popupService;
		}

		//partial void OnSiteChanged(string? oldValue, string newValue)
		//{
		//if (newValue.Equals(newSiteText))
		//{
		//CloseSitePicker();
		//DisplayPopup();
		//	}
		//}

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
        public async Task AddFlightEntryAsync()
        {
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			string[] _dateTime = Date.Split(' ');
            if (DateTime.TryParse($"{_dateTime[0]} {Time}", out DateTime result))
            {
                FlightEntryModel flightModel = new FlightEntryModel()
                {
                    DateTime = result,
                    Site = this.Site,
                    Glider = this.Glider,
                    FlightCount = this.FlightCount,
                    Hours = this.Hours,
                    Minutes = this.Minutes,
                    OmitFromTotals = this.OmitFromTotals,
                    Notes = this.Notes
                };
                if(await _addFlightEntryModel.AddFlightEntry(flightModel))
                {
					string message = "Flight Added";
					var toast = Toast.Make(message);
					await toast.Show(cancellationTokenSource.Token);
				}
                else
                {
					string message = "Failed To Add Flight To Database";
					var toast = Toast.Make(message);
					await toast.Show(cancellationTokenSource.Token);
				}
			}
			else
			{
				string message = "Failed To Parse DateTime, Flight Was Not Added";
				var toast = Toast.Make(message);
				await toast.Show(cancellationTokenSource.Token);
			}
			await Shell.Current.GoToAsync("..");
		}

		public async Task DisplayPopup(EntryPopupType entryPopupType)
		{
            try
            {
                switch (entryPopupType)
                {
                    case EntryPopupType.Site:
						//string? newSite = Convert.ToString(await _popupService.ShowPopupAsync<UserEntryPopupViewModel>(onPresenting: viewModel => viewModel.EntryLabel = "Site:"));

						//var popup = new UserEntryPopup();
						//if (popup.BindingContext is UserEntryPopupViewModel vm)
						//	vm.EntryLabel = "Site:";

						//string? newSite = await _popupService.ShowPopupAsync<string>(popup);

						//if (!string.IsNullOrEmpty(newSite))
						//{
						//	Sites.Add(newSite);
						//	Site = newSite;
						//}

						//UserEntryPopupView view = new UserEntryPopupView();

						await _popupService.ShowPopupAsync<UserEntryPopupViewModel>(Shell.Current);

						break;

                    case EntryPopupType.Glider:
						//string? newGlider = Convert.ToString(await _popupService.ShowPopupAsync<UserEntryPopupViewModel>(onPresenting: viewModel => viewModel.EntryLabel = "Glider:"));

						await _popupService.ShowPopupAsync<UserEntryPopupViewModel>(Shell.Current);


						//var popup = new UserEntryPopup();
						//if (popup.BindingContext is UserEntryPopupViewModel vm)
						//	vm.EntryLabel = "Glider:";

						//string? newGlider = await _popupService.ShowPopupAsync<string>(popup);

						//if (!string.IsNullOrEmpty(newGlider))
						//{
						//	Gliders.Add(newGlider);
						//	Glider = newGlider;
						//}
						break;
				}
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
		}
	}
}
