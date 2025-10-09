using GlideLog.ViewModels;

namespace GlideLog.Views;

public partial class FlightListView : ContentPage
{
	FlightListViewModel _flightListViewModel;

	public FlightListView(FlightListViewModel flightListViewModel)
	{
		InitializeComponent();
		BindingContext = flightListViewModel;
		_flightListViewModel = flightListViewModel;
	}

	protected override async void OnAppearing()
	{
		await _flightListViewModel.OnAppearingAsync();
	}
}