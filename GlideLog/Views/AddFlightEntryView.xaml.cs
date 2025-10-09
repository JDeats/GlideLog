using GlideLog.ViewModels;

namespace GlideLog.Views;

public partial class AddFlightEntryView : ContentPage
{
	private AddFlightEntryViewModel _addFlightEntryViewModel;
	public AddFlightEntryView(AddFlightEntryViewModel addFlightEntryViewModel)
	{
		InitializeComponent();
		BindingContext = addFlightEntryViewModel;
		_addFlightEntryViewModel = addFlightEntryViewModel;
	}

	private async void SitePicker_Unfocused(object sender, FocusEventArgs e)
	{
		await _addFlightEntryViewModel.SitePickerClosed();
	}

	private async void GliderPicker_Unfocused(object sender, FocusEventArgs e)
	{
		await _addFlightEntryViewModel.GliderPickerClosed();
	}
}