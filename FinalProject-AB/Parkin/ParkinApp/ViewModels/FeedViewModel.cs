using System.Windows.Input;
using ParkinApp.Services;

namespace ParkinApp.ViewModels;

public class FeedViewModel : BaseViewModel
{
    private readonly ParkinWebAPIProxy _proxy;

    public string WelcomeText => $"Welcome back, @{_proxy.CurrentUser?.UserName}!";

    public ICommand LogoutCommand { get; }

    public FeedViewModel(ParkinWebAPIProxy proxy)
    {
        _proxy = proxy;
        LogoutCommand = new Command(OnLogout);
    }

    private async void OnLogout()
    {
        // מחיקת הזיכרון המקומי והחזרה למסך ההתחברות
        Preferences.Default.Remove("SavedEmail");
        Preferences.Default.Remove("SavedPassword");
        _proxy.CurrentUser = null;

        await Shell.Current.GoToAsync("//LoginView");
    }
}