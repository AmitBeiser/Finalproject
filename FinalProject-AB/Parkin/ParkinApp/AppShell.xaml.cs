namespace ParkinApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        Routing.RegisterRoute("RegisterView", typeof(Views.RegisterView));
        Routing.RegisterRoute("MyVehiclesView", typeof(Views.MyVehiclesView));
        Routing.RegisterRoute("AddEditVehicleView", typeof(Views.AddEditVehicleView));
        InitializeComponent();
    }
}