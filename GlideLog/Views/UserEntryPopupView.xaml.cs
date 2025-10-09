using CommunityToolkit.Maui.Views;
using GlideLog.ViewModels;

namespace GlideLog.Views;

public partial class UserEntryPopupView : ContentView
{

	public UserEntryPopupView(UserEntryPopupViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}