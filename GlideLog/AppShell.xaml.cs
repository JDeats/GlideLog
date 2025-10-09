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
    }
}
