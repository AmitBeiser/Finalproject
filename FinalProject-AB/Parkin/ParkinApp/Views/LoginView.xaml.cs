using ParkinApp.ViewModels;

namespace ParkinApp.Views;

public partial class LoginView : ContentPage
{
    private readonly LoginViewModel _vm;

    public LoginView(LoginViewModel vm)
    {
        _vm = vm;
        BindingContext = vm;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.CheckAutoLoginAsync();
    }
}