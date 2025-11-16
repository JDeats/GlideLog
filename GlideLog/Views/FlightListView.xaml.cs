using GlideLog.ViewModels;

namespace GlideLog.Views;

public partial class FlightListView : ContentPage
{
	readonly FlightListViewModel _flightListViewModel;
	private int _scrollPosition = 0;

	public FlightListView(FlightListViewModel flightListViewModel)
	{
		InitializeComponent();
		BindingContext = flightListViewModel;
		_flightListViewModel = flightListViewModel;
		FlightCollectionView.Scrolled += OnCollectionViewScrolled;
		_flightListViewModel.ScrollToItemAction = (_scrollPosition) =>
		{
			FlightCollectionView.ScrollTo(_scrollPosition, position: ScrollToPosition.Center, animate: false);
		};
	}

	private void OnCollectionViewScrolled(object? sender, ItemsViewScrolledEventArgs e)
	{
		_scrollPosition = e.FirstVisibleItemIndex;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is FlightListViewModel viewModel)
		{
			if (_scrollPosition > 0 && _scrollPosition < viewModel.Flights.Count)
			{
				viewModel.ScrollPosition = _scrollPosition;
				viewModel.ScrollToItemAction = (_scrollPosition) =>
				{
					FlightCollectionView.ScrollTo(_scrollPosition, position: ScrollToPosition.Start, animate: false);
				};
			}

			await _flightListViewModel.OnAppearingAsync();
		}
	}
}