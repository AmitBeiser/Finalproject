using ParkinApp.ViewModels;

namespace ParkinApp.Views;

public partial class AddEditVehicleView : ContentPage
{
    public AddEditVehicleView(AddEditVehicleViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }
}