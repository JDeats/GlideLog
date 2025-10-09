using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GlideLog.ViewModels
{
	public partial class UserEntryPopupViewModel : ObservableObject
	{
		//private NewSitePopupModel _newSitePopupModel;
		private readonly IPopupService _popupService;

		public enum EntryPopupType
		{
			Site,
			Glider
		}

		[ObservableProperty]
		public partial string EntryLabel { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string UserText { get; set; } = string.Empty;

		public IPopupService PopupService => _popupService;

		public UserEntryPopupViewModel(IPopupService popupService)
		{
			//_newSitePopupModel = newSitePopupModel;
			_popupService = popupService;
		}

		[RelayCommand]
		async Task OnSave()
		{
			await _popupService.ClosePopupAsync(Shell.Current, UserText);
		}

		[RelayCommand]
		async Task OnCancel()
		{
			await _popupService.ClosePopupAsync(Shell.Current);
		}
	}
}
