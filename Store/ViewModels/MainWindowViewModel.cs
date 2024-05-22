using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Store.Context;
using Store.Models;
using Store.Services;

namespace Store.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private IQueryable<Product>? _query;
    private ObservableCollection<Product>? _items;
    private ObservableCollection<string>? _productNames;
    private ObservableCollection<Product>? _productsProvider;
    private int _selectedIndexSort;
    private int? _selectedIndexFilter;
    private string? _searchText;
    private Product? _selectedProductFilter;
    private Product? _selectedProductList;
    private bool _isAdmin;
    private int _currentUnitCount;
    private ObservableAsPropertyHelper<int> _unitCount;
    private string? _error;
    private string? _role;
    public MainWindowViewModel()
    {
        AddShowDialog = new Interaction<ProductFormWindowViewModel, Product?>();

        BasketShow = new Interaction<BasketWindowViewModel, Unit>();

        ExitWindowShow = new Interaction<SingInWindowViewModel, Unit>();

        var isValidObservable =
            this.WhenAnyValue(x => x.SelectedProductList)
                .Select( x => SelectedProductList != null);

        AddCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            ProductFormWindowViewModel productFormWindowViewModel = new ProductFormWindowViewModel();

            Product? product = await AddShowDialog.Handle(productFormWindowViewModel);

            if (product != null)
            {
                try
                {
                    await Helper.AddProductAsync(product);
                    Helper.Database = new StoreContext();
                    LoadDataToWindow();
                }
                catch (Exception e)
                {
                    Error = e.ToString();
                }
            }
        });
        
        UpdateCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            ProductFormWindowViewModel productFormWindowViewModel = new ProductFormWindowViewModel(SelectedProductList!);

            Product? product = await AddShowDialog.Handle(productFormWindowViewModel);

            if (product != null)
            {
                try
                {
                    await Helper.UpdateProductAsync(product);
                    LoadDataToWindow();
                }
                catch (Exception e)
                {
                    Error = e.ToString();
                }
            }
        },isValidObservable);

        DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                await Helper.DeleteProductAsync(SelectedProductList!.Productid);
                LoadDataToWindow();
            }
            catch (Exception e)
            {
                Error = e.ToString();
            }
        },isValidObservable); 
        
        BuyCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (SelectedProductList!.IsAvailable)
            {
                await Helper.AddProductToBasketAsync(SelectedProductList.Productid);
            }
        });

        ExitCommand = ReactiveCommand.CreateFromTask( async () =>
        {
            SingInWindowViewModel singInWindowViewModel = new SingInWindowViewModel();
            
            await ExitWindowShow.Handle(singInWindowViewModel);
        });
        
        BasketCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            BasketWindowViewModel basketWindowViewModel = new BasketWindowViewModel();

            await BasketShow.Handle(basketWindowViewModel);
        });
        
        this.WhenAnyValue(x => x.Items.Count)
            .Select(x => Query.ToList().Count)
            .ToProperty(this, x => x.UnitCount, out _unitCount);
        
        this.WhenAnyValue(x => x.Items.Count)
            .Select(x => Query.ToList().Count)
            .ToProperty(this, x => x.UnitCount, out _unitCount);
        
        /*this.WhenAnyValue(
                x => x.SearchText,
                x => x.SelectedIndexSort,
                x => x.SelectedProductFilter)
            .Subscribe( x => SetList());*/
        
        RxApp.MainThreadScheduler.Schedule(LoadDataToWindow);
    }
    public ICommand AddCommand { get; }
    public ICommand UpdateCommand { get; }
    public ICommand BuyCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand BasketCommand { get; }
    public ICommand ExitCommand { get; }
    public Interaction<ProductFormWindowViewModel, Product?> AddShowDialog { get; }
    public Interaction<BasketWindowViewModel, Unit> BasketShow { get; }
    public Interaction<SingInWindowViewModel, Unit> ExitWindowShow { get; }
    public string Role
    {
        get => IsAdmin ? "Admin" : "Guest";
        set => this.RaiseAndSetIfChanged(ref _role, value);
    }
    public string Error
    {
        get => _error!;
        set => this.RaiseAndSetIfChanged(ref _error, value);
    }
    public int CurrentUnitCount
    {
        get => _currentUnitCount;
        set => this.RaiseAndSetIfChanged(ref _currentUnitCount, value);
    }
    public int UnitCount => _unitCount.Value;
    public bool IsAdmin
    {
        get => _isAdmin;
        set => this.RaiseAndSetIfChanged(ref _isAdmin, value);
    }
    public Product? SelectedProductFilter
    {
        get => _selectedProductFilter;
        set => this.RaiseAndSetIfChanged(ref _selectedProductFilter!, value);
    }
    public Product? SelectedProductList
    {
        get => _selectedProductList;
        set => this.RaiseAndSetIfChanged(ref _selectedProductList, value);
    }
    public string? SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }
    public int SelectedIndexSort
    {
        get => _selectedIndexSort;
        set => this.RaiseAndSetIfChanged(ref _selectedIndexSort, value);
    }
    public int? SelectedIndexFilter
    {
        get => _selectedIndexFilter;
        set => this.RaiseAndSetIfChanged(ref _selectedIndexFilter, value);
    }
    public ObservableCollection<string>? ProductNames
    {
        get => _productNames;
        set => this.RaiseAndSetIfChanged(ref _productNames, value);
    }
    public ObservableCollection<Product> Items
    {
        get => _items!;
        set => this.RaiseAndSetIfChanged(ref _items, value);
    } 
    public ObservableCollection<Product> ProductsProvider
    {
        get => _productsProvider!;
        set => this.RaiseAndSetIfChanged(ref _productsProvider, value);
    }
    private IQueryable<Product> Query
    {
        get => _query!;
        set => this.RaiseAndSetIfChanged(ref _query, value);
    }
    private async void LoadDataToWindow()
    {
        try
        {
            Query = Helper.Database.Products;
            Items = new ObservableCollection<Product>(await Query.ToListAsync());
            ProductNames = new ObservableCollection<string>(Query.ToList().SelectMany( x => new []{x.Name, x.Provider}));
            ProductsProvider = new ObservableCollection<Product>(SetDataToFilterList(Query));
            SelectedProductFilter = ProductsProvider[0];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Error = e.ToString();
        }
    }
    private List<Product> SetDataToFilterList(IQueryable<Product> query)
    {
        try
        {
            var list = new List<Product>{ new() { Provider = "Все производители" } };
            list.AddRange(query);
            List<Product> temp = list
                .GroupBy(x => x.Provider)
                .Select(x => x.First())
                .ToList();
            return temp;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Error = e.ToString();
            return null!;
        }
    }
    /*private void SetList()
    {
        try
        {
            Items = new ObservableCollection<Product>(ListTransform());

            CurrentUnitCount = Items.Count;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Error = e.ToString();
        }
    }*/
    private List<Product> ListTransform()
    {
        try 
        {
            IQueryable<Product> products = SelectedIndexFilter switch
            {
                0 => Query,
                _ => Query.Where(x => x.Provider == SelectedProductFilter!.Provider)
            };

            products = !string.IsNullOrEmpty(SearchText)
                ? products.Where(x => 
                    x.Name.ToLower().Contains(SearchText.ToLower()) || 
                    x.Provider.ToLower().Contains(SearchText.ToLower()))
                : products;   
        
            products = SelectedIndexSort switch
            {
                0 => products,
                1 => products.OrderByDescending(x => x.Cost),
                2 => products.OrderBy(x => x.Cost),
                _ => products
            };
            return products.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Error = e.ToString();
            throw;
        }
    }
}