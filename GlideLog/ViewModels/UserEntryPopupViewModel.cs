using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GlideLog.ViewModels
{
	public partial class UserEntryPopupViewModel : ObservableObject, IQueryAttributable
	{
		private readonly IPopupService _popupService;

		public enum EntryPopupType
		{
			Site,
			Glider
		}

		[ObservableProperty]
		public partial string EntryLabel { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string PlaceholderText { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string UserText { get; set; } = string.Empty;

		public IPopupService PopupService => _popupService;

		public UserEntryPopupViewModel(IPopupService popupService)
		{
			_popupService = popupService;
		}

		public void ApplyQueryAttributes(IDictionary<string, object> query)
		{
			if (query.TryGetValue(nameof(EntryLabel), out var val) && val is string s)
				EntryLabel = s;

			PlaceholderText = $"Enter New {EntryLabel[..^1]}...";
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
