using System.Collections.ObjectModel;
using System.Windows.Input;
using ParkinApp.Models;
using ParkinApp.Services;

namespace ParkinApp.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private string fullName = "";
    public string FullName
    {
        get => fullName;
        set { fullName = value; OnPropertyChanged(); }
    }

    private string userName = "";
    public string UserName
    {
        get => userName;
        set { userName = value; OnPropertyChanged(); }
    }

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

    private string photoURL = "dotnet_bot.png";
    public string PhotoURL
    {
        get => photoURL;
        set { photoURL = value; OnPropertyChanged(); }
    }

    private string localPhotoPath = "";
    public string LocalPhotoPath
    {
        get => localPhotoPath;
        set { localPhotoPath = value; OnPropertyChanged(); }
    }

    private string errorMsg = "";
    public string ErrorMsg
    {
        get => errorMsg;
        set { errorMsg = value; OnPropertyChanged(); }
    }

    public ObservableCollection<Vehicle> Vehicles { get; set; } = new ObservableCollection<Vehicle>();

    public string VehiclesCountText => $"Vehicles added: {Vehicles.Count} (At least 1 required)";

    public ICommand UploadPhotoCommand { get; }
    public ICommand ManageVehiclesCommand { get; }
    public ICommand RegisterCommand { get; }
    public ICommand BackToLoginCommand { get; }

    private readonly ParkinWebAPIProxy _proxy;

    public RegisterViewModel(ParkinWebAPIProxy proxy)
    {
        _proxy = proxy;
        UploadPhotoCommand = new Command(OnUploadPhoto);
        ManageVehiclesCommand = new Command(OnManageVehicles);
        RegisterCommand = new Command(OnRegister);
        BackToLoginCommand = new Command(OnBackToLogin);
    }

    public void RefreshVehiclesCount()
    {
        OnPropertyChanged(nameof(VehiclesCountText));
    }

    private async void OnUploadPhoto()
    {
        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Select a profile photo"
            });

            if (result != null)
            {
                LocalPhotoPath = result.FullPath;
                PhotoURL = result.FullPath;
            }
        }
        catch (Exception ex)
        {
            ErrorMsg = $"Could not load image: {ex.Message}";
        }
    }

    private async void OnManageVehicles()
    {
        var navParam = new Dictionary<string, object>
        {
            { "vehiclesList", Vehicles },
            { "registerVm", this }
        };
        await Shell.Current.GoToAsync("MyVehiclesView", navParam);
    }

    private async void OnRegister()
    {
        if (string.IsNullOrWhiteSpace(FullName) ||
            string.IsNullOrWhiteSpace(UserName) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password))
        {
            ErrorMsg = "Please fill in all personal details.";
            return;
        }

        if (Vehicles.Count == 0)
        {
            ErrorMsg = "You must add at least one vehicle before registering.";
            return;
        }
        ErrorMsg = "";

        var newUser = new User
        {
            FullName = FullName.Trim(),
            UserName = UserName.Trim(),
            Email = Email.Trim(),
            Password = Password,
            ProfileImagePath = LocalPhotoPath,
            Vehicles = Vehicles.ToList()
        };

        var (createdUser, error) = await _proxy.RegisterAsync(newUser);

        if (createdUser == null)
        {
            ErrorMsg = error ?? "Registration failed.";
            return;
        }

// שמירת פרטי התחברות בזיכרון המכשיר עבור כניסה אוטומטית
        Preferences.Default.Set("SavedEmail", createdUser.Email);
        Preferences.Default.Set("SavedPassword", createdUser.Password);

// מעבר למסך הראשי (הסימן // מאפס את היסטוריית הניווט כדי שלא יחזרו אחורה להרשמה)
        await Shell.Current.GoToAsync("//FeedView");
        }

    private async void OnBackToLogin()
    {
        await Shell.Current.GoToAsync("..");
    }
}