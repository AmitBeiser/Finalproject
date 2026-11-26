using ParkinApp.ViewModels;

namespace ParkinApp.Views;

public partial class RegisterView : ContentPage
{
    public RegisterView(RegisterViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }
}