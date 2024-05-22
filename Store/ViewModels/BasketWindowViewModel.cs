using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Store.Models;
using Store.Services;


namespace Store.ViewModels;

public class BasketWindowViewModel : ViewModelBase
{
    private IQueryable<Basket>? _query;
    private ObservableCollection<Product>? _items;
    private Product? _selectedProductList;
    private bool _isAdmin;
    private string? _error;
    private string? _role;

    public BasketWindowViewModel()
    {
        RxApp.MainThreadScheduler.Schedule(LoadDataToWindow);

        DeleteCommand = ReactiveCommand.Create(() =>
        {
            try
            {
                Basket basket = Helper.Database.Baskets.First(x => SelectedProductList!.Productid == x.Productid);
                Helper.Database.Baskets.Remove(basket);
                Helper.Database.SaveChanges();
                LoadDataToWindow();
            }
            catch (Exception e)
            {
                Error = e.ToString();
            }
        });
    }

    public ICommand DeleteCommand { get; }
    
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

    public bool IsAdmin
    {
        get => _isAdmin;
        set => this.RaiseAndSetIfChanged(ref _isAdmin, value);
    }

    public Product? SelectedProductList
    {
        get => _selectedProductList;
        set => this.RaiseAndSetIfChanged(ref _selectedProductList, value);
    }

    public ObservableCollection<Product> Items
    {
        get => _items!;
        set => this.RaiseAndSetIfChanged(ref _items, value);
    }

    private void LoadDataToWindow()
    {
        try
        {
            _query = Helper.Database.Baskets.Include(x => x.Product);
            Items = new ObservableCollection<Product>(_query.Select(x => x.Product));
        }
        catch (Exception e)
        {
            Error = e.ToString();
        }
    }
}   