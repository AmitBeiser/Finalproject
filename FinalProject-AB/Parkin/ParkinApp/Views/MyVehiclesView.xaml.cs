using ParkinApp.ViewModels;

namespace ParkinApp.Views;

public partial class MyVehiclesView : ContentPage
{
    public MyVehiclesView(MyVehiclesViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }
}