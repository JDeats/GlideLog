using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GlideLog.Models;
using System.Collections.ObjectModel;

namespace GlideLog.ViewModels
{
	public partial class TotalsViewModel : ObservableObject
	{
		private TotalsModel _totalsModel;
		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		[ObservableProperty]
		int flightCount;

		[ObservableProperty]
		int hours;

		[ObservableProperty]
		int minutes;

		[ObservableProperty]
		ObservableCollection<TotalsByMonthModel> totalsByMonth;

		public TotalsViewModel(TotalsModel totalsModel)
		{
			_totalsModel = totalsModel;
			TotalsByMonth = new ObservableCollection<TotalsByMonthModel>();
		}

		[RelayCommand]
		public async Task Appearing()
		{
			try
			{
				await _totalsModel.GetFlightEntriesAsync();
				FlightCount = await _totalsModel.GetTotalFlightsAsync();
				Tuple<int, int> time = await _totalsModel.GetTotalFlightsHoursAsync();
				var totalsByMonthDic = await _totalsModel.GetTotalsByMonthAsync();
				foreach (var kvp in totalsByMonthDic)
				{
					TotalsByMonth.Add(new TotalsByMonthModel() { Date = kvp.Key, TotalHours = kvp.Value.Item1, TotalMinutes = kvp.Value.Item2, TotalFlights = kvp.Value.Item3 });
				}
				Hours = time.Item1;
				Minutes = time.Item2;
			}
			catch (Exception ex)
			{
				string message = $"Failed To Load Flights From the Database: {ex.Message}";
				var toast = Toast.Make(message);
				await toast.Show(_cancellationTokenSource.Token);
			}
		}

	}
}
