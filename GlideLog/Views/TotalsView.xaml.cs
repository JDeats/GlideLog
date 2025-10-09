using GlideLog.ViewModels;

namespace GlideLog.Views;

public partial class TotalsView : ContentPage
{
	private readonly TotalsViewModel _totalsViewModel;

	public TotalsView(TotalsViewModel totalsViewModel)
	{
		InitializeComponent();
		BindingContext = totalsViewModel;
		_totalsViewModel = totalsViewModel;
	}

	protected override async void OnAppearing()
	{
		await _totalsViewModel.OnAppearingAsync();
	}
}