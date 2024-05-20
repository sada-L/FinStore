using System.Reactive;
using System.Threading.Tasks;
using Store.ViewModels;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Store.Models;

namespace Store.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(action =>
            action(ViewModel!.AddShowDialog.RegisterHandler(DoAddShowDialogAsync)));
        
        this.WhenActivated(action =>
            action(ViewModel!.BasketShow.RegisterHandler(DoBasketShow)));
        
        this.WhenActivated(action =>
            action(ViewModel!.ExitWindowShow.RegisterHandler(DoExitShow)));
    }
    
    private async Task DoAddShowDialogAsync(InteractionContext<AddProductWindowViewModel, Product?> interactionContext)
    {
        AddProductWindow addProductWindow = new AddProductWindow();
        addProductWindow.DataContext = interactionContext.Input;

        Product? product = await addProductWindow.ShowDialog<Product?>(this);
        interactionContext.SetOutput(product);
    }
    private void DoBasketShow(InteractionContext<BasketWindowViewModel, Unit> interactionContext)
    {
        BasketWindow basketWindow = new BasketWindow
        {
            DataContext = interactionContext.Input
        };

        basketWindow.Show(this);
    }
    
    private void DoExitShow(InteractionContext<SingInWindowViewModel, Unit> interactionContext)
    {
        SingInWindow singInWindow = new SingInWindow
        {
            DataContext = interactionContext.Input
        };
        singInWindow.Show();
        
        Close();
    }
}