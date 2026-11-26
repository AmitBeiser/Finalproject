using System.Collections.ObjectModel;
using System.Windows.Input;
using ParkinApp.Models;

namespace ParkinApp.ViewModels;

[QueryProperty(nameof(Vehicles), "vehiclesList")]
[QueryProperty(nameof(RegisterVm), "registerVm")]
public class MyVehiclesViewModel : BaseViewModel
{
    private ObservableCollection<Vehicle> vehicles = new();
    public ObservableCollection<Vehicle> Vehicles
    {
        get => vehicles;
        set { vehicles = value; OnPropertyChanged(); }
    }

    // אני שומר את הסיכוי של מסך ניהול הרכבים יקבל את ה-RegisterViewModel כדי שאוכל לקרוא לו ולרענן את ספירת הרכבים
    public RegisterViewModel? RegisterVm { get; set; }

    public ICommand AddVehicleCommand { get; }
    public ICommand EditVehicleCommand { get; }
    public ICommand DeleteVehicleCommand { get; }
    public ICommand DoneCommand { get; }

    public MyVehiclesViewModel()
    {
        AddVehicleCommand = new Command(OnAddVehicle);
        EditVehicleCommand = new Command<Vehicle>(OnEditVehicle);
        DeleteVehicleCommand = new Command<Vehicle>(OnDeleteVehicle);
        DoneCommand = new Command(OnDone);
    }

    private async void OnAddVehicle()
    {
        var navParam = new Dictionary<string, object>
        {
            { "vehiclesList", Vehicles }
        };
        await Shell.Current.GoToAsync("AddEditVehicleView", navParam);
    }

    private async void OnEditVehicle(Vehicle vehicleToEdit)
    {
        if (vehicleToEdit == null) return;

        var navParam = new Dictionary<string, object>
        {
            { "vehiclesList", Vehicles },
            { "vehicleToEdit", vehicleToEdit }
        };
        await Shell.Current.GoToAsync("AddEditVehicleView", navParam);
    }

    private void OnDeleteVehicle(Vehicle vehicleToDelete)
    {
        if (vehicleToDelete == null) return;

        Vehicles.Remove(vehicleToDelete);
        RegisterVm?.RefreshVehiclesCount();
    }

    private async void OnDone()
    {
        RegisterVm?.RefreshVehiclesCount();
        await Shell.Current.GoToAsync("..");
    }
}