using GlideLog.ViewModels;
using GlideLog.Views;

namespace GlideLog
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

			Routing.RegisterRoute(nameof(AddFlightEntryView), typeof(AddFlightEntryView));
			Routing.RegisterRoute(nameof(EditFlightEntryView), typeof(EditFlightEntryView));
		}

		private async void ClearFlightsFlyoutItem_Clicked(object sender, EventArgs e)
		{
			try
			{
				bool answer = await DisplayAlert("Confirm", "Do you really want to delete all your flight logs?", "Yes", "No");
				if (answer)
				{
					var viewModel = App.Current?.Handler.MauiContext?.Services.GetService<FlightListViewModel>();
					if (viewModel != null)
					{
						await viewModel!.ClearFlights();
					}
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
			}
		}

		private async void ImportLogFlyoutItem_Clicked(object sender, EventArgs e)
		{
			try
			{
				var viewModel = App.Current?.Handler.MauiContext?.Services.GetService<FlightListViewModel>();
				if (viewModel != null)
				{
					await viewModel!.ImportAsync();
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
			}
		}

		private async void ExportLogFlyoutItem_Clicked(object sender, EventArgs e)
		{
			try
			{
				var viewModel = App.Current?.Handler.MauiContext?.Services.GetService<FlightListViewModel>();
				if (viewModel != null)
				{
					await viewModel!.ExportAsync();
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
			}
		}
	}
}
