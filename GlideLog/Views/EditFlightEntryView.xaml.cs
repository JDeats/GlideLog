using GlideLog.ViewModels;

namespace GlideLog.Views;

public partial class EditFlightEntryView : ContentPage
{
	private readonly EditFlightEntryViewModel _editFlightEntryViewModel;

	public EditFlightEntryView(EditFlightEntryViewModel editFlightEntryViewModel)
	{
		InitializeComponent();
		BindingContext = editFlightEntryViewModel;
		_editFlightEntryViewModel = editFlightEntryViewModel;
	}

	protected override async void OnAppearing()
	{
		await _editFlightEntryViewModel.OnAppearingAsync();
	}

	private async void SitePicker_Unfocused(object sender, FocusEventArgs e)
	{
		await _editFlightEntryViewModel.SitePickerClosed();
	}

	private async void GliderPicker_Unfocused(object sender, FocusEventArgs e)
	{
		await _editFlightEntryViewModel.GliderPickerClosed();
	}
}