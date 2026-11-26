using System.Windows.Input;
using ParkinApp.Services;

namespace ParkinApp.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly ParkinWebAPIProxy _proxy;

    private string email = "";
    public string Email
    {
        get => email;
        set { email = value; OnPropertyChanged(); }
    }

    private string password = "";
    public string Password
    {
        get => password;
        set { password = value; OnPropertyChanged(); }
    }

    private string errorMsg = "";
    public string ErrorMsg
    {
        get => errorMsg;
        set { errorMsg = value; OnPropertyChanged(); }
    }

    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }
    public ICommand ForgotPasswordCommand { get; }

    public LoginViewModel(ParkinWebAPIProxy proxy)
    {
        _proxy = proxy;
        LoginCommand = new Command(OnLogin);
        RegisterCommand = new Command(OnRegister);
        ForgotPasswordCommand = new Command(OnForgotPassword);
    }

    // בדיקה אוטומטית כשהמסך עולה: האם המשתמש כבר התחבר בעבר?
    public async Task CheckAutoLoginAsync()
    {
        string savedEmail = Preferences.Default.Get("SavedEmail", "");
        string savedPassword = Preferences.Default.Get("SavedPassword", "");

        if (!string.IsNullOrEmpty(savedEmail) && !string.IsNullOrEmpty(savedPassword))
        {
            var (user, _) = await _proxy.LoginAsync(savedEmail, savedPassword);
            if (user != null)
            {
                await Shell.Current.GoToAsync("//FeedView");
            }
        }
    }

    private async void OnLogin()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMsg = "Please enter both email and password.";
            return;
        }

        ErrorMsg = "";
        var (user, error) = await _proxy.LoginAsync(Email.Trim(), Password);

        if (user == null)
        {
            ErrorMsg = error ?? "Invalid email or password.";
            return;
        }

        Preferences.Default.Set("SavedEmail", user.Email);
        Preferences.Default.Set("SavedPassword", user.Password);

        await Shell.Current.GoToAsync("//FeedView");
    }

    private async void OnRegister()
    {
        await Shell.Current.GoToAsync("RegisterView");
    }

    private async void OnForgotPassword()
    {
        await Application.Current!.MainPage!.DisplayAlert("Reset Password", "Password reset link sent to your email.", "OK");
    }
}