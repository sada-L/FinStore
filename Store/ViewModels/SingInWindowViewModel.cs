using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;

namespace Store.ViewModels;

public class SingInWindowViewModel : ViewModelBase
{
    private string _code = string.Empty;
    
    public SingInWindowViewModel()
    {
        OpenWindowShow = new  Interaction<MainWindowViewModel, Unit>();
        
        OpenWindow = ReactiveCommand.CreateFromTask( async () =>
        {
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();

            mainWindowViewModel.IsAdmin = (Code ?? "") == "123";
            
            await OpenWindowShow.Handle(mainWindowViewModel);
        });
    }

   public ICommand OpenWindow { get; }
   
   public Interaction<MainWindowViewModel, Unit> OpenWindowShow { get; }
    
    public string Code
    {
        get => _code;
        set => this.RaiseAndSetIfChanged(ref _code, value);
    }
}   