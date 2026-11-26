using System.Collections.ObjectModel;
using System.Windows.Input;
using ParkinApp.Models;
using ParkinApp.Services;

namespace ParkinApp.ViewModels;

[QueryProperty(nameof(Vehicles), "vehiclesList")]
[QueryProperty(nameof(VehicleToEdit), "vehicleToEdit")]
public class AddEditVehicleViewModel : BaseViewModel
{
    public ObservableCollection<Vehicle> Vehicles { get; set; } = new();

    private Vehicle? vehicleToEdit;
    public Vehicle? VehicleToEdit
    {
        get => vehicleToEdit;
        set
        {
            vehicleToEdit = value;
            if (vehicleToEdit != null)
            {
                // מצב עריכה: ממלאים את השדות בנתונים הקיימים של הרכב
                PageTitle = "Edit Vehicle";
                ModelName = vehicleToEdit.ModelName;
                LicensePlate = vehicleToEdit.LicensePlate;
                if (!string.IsNullOrEmpty(vehicleToEdit.VehicleImagePath))
                {
                    PhotoURL = vehicleToEdit.VehicleImagePath;
                }
            }
        }
    }

    private string pageTitle = "Add New Vehicle";
    public string PageTitle
    {
        get => pageTitle;
        set { pageTitle = value; OnPropertyChanged(); }
    }

    private string modelName = "";
    public string ModelName
    {
        get => modelName;
        set { modelName = value; OnPropertyChanged(); }
    }

    private string licensePlate = "";
    public string LicensePlate
    {
        get => licensePlate;
        set { licensePlate = value; OnPropertyChanged(); }
    }

    private string photoURL = "dotnet_bot.png";
    public string PhotoURL
    {
        get => photoURL;
        set { photoURL = value; OnPropertyChanged(); }
    }

    private string errorMsg = "";
    public string ErrorMsg
    {
        get => errorMsg;
        set { errorMsg = value; OnPropertyChanged(); }
    }

    public ICommand PickPhotoCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private readonly ParkinWebAPIProxy _proxy;

    public AddEditVehicleViewModel(ParkinWebAPIProxy proxy)
    {
        _proxy = proxy;
        PickPhotoCommand = new Command(OnPickPhoto);
        SaveCommand = new Command(OnSave);
        CancelCommand = new Command(OnCancel);
    }

    private async void OnPickPhoto()
    {
        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Select a vehicle photo"
            });

            if (result != null)
            {
                PhotoURL = result.FullPath;
            }
        }
        catch (Exception ex)
        {
            ErrorMsg = $"Could not load image: {ex.Message}";
        }
    }

    private async void OnSave()
    {
        if (string.IsNullOrWhiteSpace(ModelName) || string.IsNullOrWhiteSpace(LicensePlate))
        {
            ErrorMsg = "Please enter both vehicle model and license plate.";
            return;
        }

        if (VehicleToEdit != null)
        {
            // עדכון רכב קיים
            VehicleToEdit.ModelName = ModelName;
            VehicleToEdit.LicensePlate = LicensePlate;
            VehicleToEdit.VehicleImagePath = PhotoURL;

            // טריק חשוב: הצבה מחדש באותו אינדקס גורמת ל-ObservableCollection לשדר עדכון למסך הקודם
            int index = Vehicles.IndexOf(VehicleToEdit);
            if (index >= 0)
            {
                Vehicles[index] = VehicleToEdit;
            }
        }
        else // new vehicle
        {
            var existing = await _proxy.CheckVehicleAsync(LicensePlate.Trim());

            if (existing != null)
            {
                // הלוחית קיימת אבל הדגם שונה -> חוסמים
                if (!existing.ModelName.Trim().Equals(ModelName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    ErrorMsg = $"License plate is already registered to a different model ({existing.ModelName}).";
                    return;
                }

                // הלוחית והדגם תואמים -> מציעים שיתוף רכב!
                bool confirmShare = await Application.Current!.MainPage!.DisplayAlert(
                    "Vehicle Already Exists",
                    $"This {existing.ModelName} is already registered by {existing.Owners}.\nDo you want to share this vehicle?",
                    "Yes, Share Vehicle",
                    "Cancel");

                if (!confirmShare)
                    return;

                if (PhotoURL == "dotnet_bot.png" && !string.IsNullOrEmpty(existing.VehicleImagePath))
                {
                    PhotoURL = existing.VehicleImagePath;
                }
            }

            Vehicles.Add(new Vehicle
            {
                ModelName = ModelName.Trim(),
                LicensePlate = LicensePlate.Trim(),
                VehicleImagePath = PhotoURL
            });
        }

        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}