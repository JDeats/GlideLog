using CommunityToolkit.Maui.Views;
using GlideLog.ViewModels;

namespace GlideLog.Views;

public partial class UserEntryPopupView : ContentView
{
	//private UserEntryPopupViewModel _viewModel;

	public UserEntryPopupView(UserEntryPopupViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		//_viewModel = viewModel;
	}
}