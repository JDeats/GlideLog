using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using GlideLog.Data;
using GlideLog.Models;
using GlideLog.ViewModels;
using GlideLog.Views;

namespace GlideLog
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.UseMauiCommunityToolkit(options =>
				{
					options.SetShouldEnableSnackbarOnWindows(true);
				})
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

#if DEBUG
			builder.Logging.AddDebug();
#endif
			builder.Services.AddSingleton<FlightDatabase>();

			builder.Services.AddSingleton<FlightListView>();
			builder.Services.AddSingleton<FlightListViewModel>();
			builder.Services.AddSingleton<FlightListModel>();

			builder.Services.AddTransient<AddFlightEntryView>();
			builder.Services.AddTransient<AddFlightEntryViewModel>();
			builder.Services.AddTransient<AddFlightEntryModel>();

			builder.Services.AddSingleton<TotalsView>();
			builder.Services.AddSingleton<TotalsViewModel>();
			builder.Services.AddSingleton<TotalsModel>();

			builder.Services.AddTransient<EditFlightEntryView>();
			builder.Services.AddTransient<EditFlightEntryViewModel>();
			builder.Services.AddTransient<EditFlightEntryModel>();

			builder.Services.AddTransientPopup<UserEntryPopupView, UserEntryPopupViewModel>();

			return builder.Build();
		}
    }
}
