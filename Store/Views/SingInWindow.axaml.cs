using System.Reactive;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Store.ViewModels;

namespace Store.Views;

public partial class SingInWindow : ReactiveWindow<SingInWindowViewModel>
{
    public SingInWindow()
    {
        InitializeComponent();
        this.WhenActivated(action => 
            action(ViewModel!.OpenWindowShow.RegisterHandler(Handler)));
    }

    private void Handler(InteractionContext<MainWindowViewModel, Unit> interactionContext)
    {
        MainWindow mainWindow = new MainWindow
        {
            DataContext = interactionContext.Input
        };
        mainWindow.Show();
        
        Close();
    }
}