using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Store.ViewModels;
using Store.Views;

namespace Store;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new SingInWindow
            {
                DataContext = new SingInWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}