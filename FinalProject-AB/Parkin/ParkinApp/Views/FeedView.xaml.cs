using ParkinApp.ViewModels;

namespace ParkinApp.Views;

public partial class FeedView : ContentPage
{
    public FeedView(FeedViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }
}