using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
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
    private int _selectedIndexFilter;
    private string? _searchText;
    private Product? _selectedProductFilter;
    private Product? _selectedProductList;
    private bool _isAdmin;
    private int _currentUnitCount;
    private string? _error;
    private string? _role;

    public MainWindowViewModel()
    {
        LoadDataToWindow();
        
        AddShowDialog = new Interaction<AddProductWindowViewModel, Product?>();

        BasketShow = new Interaction<BasketWindowViewModel, Unit>();

        ExitWindowShow = new Interaction<SingInWindowViewModel, Unit>();

        AddCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            AddProductWindowViewModel addProductWindowViewModel = new AddProductWindowViewModel();

            Product? product = await AddShowDialog.Handle(addProductWindowViewModel);

            if (product != null)
            {
                try
                {
                    Helper.Database.Products.Add(product);
                    Helper.Database.SaveChanges();
                    LoadDataToWindow();
                }
                catch (Exception e)
                {
                    Error = e.ToString();
                }
            }
        });

        DeleteCommand = ReactiveCommand.Create(() =>
        {
            try
            {
                Helper.Database.Products.Remove(SelectedProductList!);
                Helper.Database.SaveChanges();
                LoadDataToWindow();
            }
            catch (Exception e)
            {
                Error = e.ToString();
            }
        }); 
        
        BuyCommand = ReactiveCommand.Create(() =>
        {
            if (SelectedProductList!.IsAvailable)
            {
                Basket basket = new Basket();
                basket.Productid = SelectedProductList!.Productid;
                Helper.Database.Baskets.Add(basket);
                Helper.Database.SaveChanges();
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
        

        this.WhenAnyValue(x => x.SearchText)
            .Subscribe(LoadWindow!); 
        
        this.WhenAnyValue(x => x.SelectedIndexSort)
            .Subscribe(LoadWindow);
        
        this.WhenAnyValue(x => x.SelectedProductFilter)
            .Subscribe(LoadWindow!);
    }
    
    public ICommand AddCommand { get; }
    
    public ICommand BuyCommand { get; }
    
    public ICommand DeleteCommand { get; }

    public ICommand BasketCommand { get; }
    public ICommand ExitCommand { get; }
    
    public Interaction<AddProductWindowViewModel, Product?> AddShowDialog { get; }

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

    public int UnitCount => _query!.Count();
    
    public bool IsAdmin
    {
        get => _isAdmin;
        set => this.RaiseAndSetIfChanged(ref _isAdmin, value);
    }

    public Product? SelectedProductFilter
    {
        get => _selectedProductFilter;
        set => this.RaiseAndSetIfChanged(ref _selectedProductFilter, value);
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
    public int SelectedIndexFilter
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

    private void LoadDataToWindow()
    {
        try
        {
            _query = Helper.Database.Products;
            Items = new ObservableCollection<Product>(_query.ToList());
            ProductNames = new ObservableCollection<string>(_query.ToList().SelectMany( x => new []{x.Name, x.Provider}));
            ProductsProvider = new ObservableCollection<Product>(SetDataToFilterList(_query)!);
        }
        catch (Exception e)
        {
            Error = e.ToString();
        }
    }
    
    private List<Product>? SetDataToFilterList(IQueryable<Product> query)
    {
        try
        {
            var list = new List<Product>{new(){Provider = "Все производители"}};
            list.AddRange(query);
            List<Product> temp = list
                .GroupBy(x => x.Provider)
                .Select(x => x.First())
                .ToList();
            return temp;
        }
        catch (Exception e)
        {
            Error = e.ToString();
            return null;
        }
    }
    
    private void LoadDataToList()
    {
        try
        {
            //init
            IEnumerable<Product> products = FilteringQuery(); // filtering

            // search text
            products = SearchByText(products);

            // sorting
            products = SortingQuery(products);
        
            Items = new ObservableCollection<Product>(products);
        
            CurrentUnitCount = Items.Count;
        }
        catch (Exception e)
        {
            Error = e.ToString();
        }
    }
    
    private IEnumerable<Product> SearchByText(IEnumerable<Product> products) =>
        !string.IsNullOrEmpty(SearchText)
            ? products.Where(x => 
                x.Name.ToLower().Contains(SearchText.ToLower()) || 
                x.Provider.ToLower().Contains(SearchText.ToLower()))
            : products;

    private IEnumerable<Product> FilteringQuery() => SelectedIndexFilter switch
    {
        0 => _query!,
        _ => _query!.Where(x => x.Provider == SelectedProductFilter!.Provider)
    };

    private IEnumerable<Product> SortingQuery(IEnumerable<Product> products) => SelectedIndexSort switch
    {
        0 => products,
        1 => products.OrderByDescending(x => x.Cost),
        2 => products.OrderBy(x => x.Cost),
        _ => products
    };
    
    private void LoadWindow(string s) => LoadDataToList();
    
    private void LoadWindow(int i) => LoadDataToList();
    
    private void LoadWindow(Product p) => LoadDataToList();
}