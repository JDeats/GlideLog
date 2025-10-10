using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GlideLog.Models;
using GlideLog.Views;
using System.Collections.ObjectModel;
using System.Globalization;

namespace GlideLog.ViewModels
{
	public partial class TotalsViewModel : ObservableObject
	{
		private readonly TotalsModel _totalsModel;
		private readonly CancellationTokenSource _cancellationTokenSource;

		[ObservableProperty]
		public partial int FlightCount { get; set; }

		[ObservableProperty]
		public partial int Hours { get; set; }

		[ObservableProperty]
		public partial int Minutes { get; set; }

		[ObservableProperty]
		public partial ObservableCollection<TotalsByGliderModel> TotalsByGlider { get; set; }

		[ObservableProperty]
		public partial ObservableCollection<TotalsByMonthModel> TotalsByMonth { get; set; }

		[ObservableProperty]
		public partial ObservableCollection<TotalsBySiteModel> TotalsBySite { get; set; }

		[ObservableProperty]
		public partial View TotalListSelection { get; set; } = new TotalsByMonthView();

		public TotalsViewModel(TotalsModel totalsModel)
		{
			_totalsModel = totalsModel;
			TotalsByMonth = [];
			TotalsBySite = [];
			_cancellationTokenSource = new();
		}

		public async Task OnAppearingAsync()
		{
			try
			{
				await _totalsModel.GetFlightEntriesAsync();
				FlightCount = await _totalsModel.GetTotalFlightsAsync();
				Tuple<int, int> time = await _totalsModel.GetTotalFlightsHoursAsync();
				Hours = time.Item1;
				Minutes = time.Item2;

				// Totals By Month
				var totalsByMonthDic = await _totalsModel.GetTotalsByMonthAsync();
				var monthlyList = totalsByMonthDic
					.Reverse()
					.Select(kvp => new TotalsByMonthModel
					{
						Date = kvp.Key,
						TotalHours = kvp.Value.Item1,
						TotalMinutes = kvp.Value.Item2,
						TotalFlights = kvp.Value.Item3
					});
				monthlyList = monthlyList.OrderByDescending(item => DateTime.ParseExact(item.Date, "MMMM yyyy", CultureInfo.InvariantCulture));
				TotalsByMonth = new ObservableCollection<TotalsByMonthModel>(monthlyList);

				// Totals By Site
				var totalsBySiteDic = await _totalsModel.GetTotalsBySiteAsync();
				var siteList = totalsBySiteDic
					.Reverse()
					.Select(kvp => new TotalsBySiteModel
					{
						Site = kvp.Key,
						TotalHours = kvp.Value.Item1,
						TotalMinutes = kvp.Value.Item2,
						TotalFlights = kvp.Value.Item3
					});
				TotalsBySite = new ObservableCollection<TotalsBySiteModel>(siteList);

				// Totals By Glider
				var totalsByGliderDic = await _totalsModel.GetTotalsByGliderAsync();
				var gliderList = totalsByGliderDic
					.Reverse()
					.Select(kvp => new TotalsByGliderModel
					{
						Glider = kvp.Key,
						TotalHours = kvp.Value.Item1,
						TotalMinutes = kvp.Value.Item2,
						TotalFlights = kvp.Value.Item3
					});
				TotalsByGlider = new ObservableCollection<TotalsByGliderModel>(gliderList);
			}
			catch (Exception ex)
			{
				string message = $"Failed To Load Flights From the Database: {ex.Message}";
				var toast = Toast.Make(message);
				await toast.Show(_cancellationTokenSource.Token);
			}
		}

		[RelayCommand]
		public void ShowByMonthView() => TotalListSelection = new TotalsByMonthView();

		[RelayCommand]
		public void ShowBySiteView() => TotalListSelection = new TotalsBySiteView();

		[RelayCommand]
		public void ShowByGliderView() => TotalListSelection = new TotalsByGliderView();

	}
}
