using System.Reactive;
using ReactiveUI;
using Store.Models;

namespace Store.ViewModels;

public class AddProductWindowViewModel : ViewModelBase
{
    private string _name = string.Empty;
    private int? _categoryid = null;
    private int? _count = null;
    private string _unit = string.Empty;
    private string _provider = string.Empty;
    private double? _cost = null;

    
    public AddProductWindowViewModel()
    {
        var isValidObservable = this.WhenAnyValue(x => x.Name, x => !string.IsNullOrEmpty(x));

        OkCommand = ReactiveCommand.Create(() => new Product
            {
                Name = Name,
                Categoryid = Categoryid,
                Count = Count,
                Unit = Unit,
                Provider = Provider,
                Cost = Cost
            }, 
            isValidObservable);
    }
    public ReactiveCommand<Unit, Product> OkCommand { get; }
    
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    public int? Categoryid
    {
        get => _categoryid;
        set => this.RaiseAndSetIfChanged(ref _categoryid, value);
    }
    public int? Count
    {
        get => _count;
        set => this.RaiseAndSetIfChanged(ref _count, value);
    }
    public string Unit
    {
        get => _unit;
        set => this.RaiseAndSetIfChanged(ref _unit, value);
    }
    public string Provider 
    {
        get => _provider;
        set => this.RaiseAndSetIfChanged(ref _provider, value);
    }
    public double? Cost
    {
        get => _cost;
        set => this.RaiseAndSetIfChanged(ref _cost, value);
    }
}