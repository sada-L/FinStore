using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ReactiveUI;
using Store.Models;
using Store.Services;

namespace Store.ViewModels;

public class ProductFormWindowViewModel : ViewModelBase
{
    private ObservableCollection<Category> _categories;
    private string _name = string.Empty;
    private Category? _category = null;
    private int? _count = null;
    private string _unit = string.Empty;
    private string _provider = string.Empty;
    private double? _cost = null;
    private string? _imageName;
    private string? _description;
    private Bitmap? _image;
   
    public ProductFormWindowViewModel()
    {
        Category = Helper.Database.Categories.First();
        _categories = new ObservableCollection<Category>(Helper.Database.Categories);
        
        var isValidObservable = this.WhenAnyValue(
            x => x.Name, 
            x => x.Category,
            x => x.Count,
            x => x.Unit,
            x => x.Provider,
            x => x.Cost,
            (name, category, count, unit, provider, cost) =>
                !string.IsNullOrEmpty(name) &&
                (category ?? _categories[0]) != null &&
                count >= 0 &&
                !string.IsNullOrEmpty(unit) &&
                !string.IsNullOrEmpty(provider) &&
                cost > 0);
        
        OkCommand = ReactiveCommand.Create( () => new Product()
        {
            Name = Name,
            Categoryid = Category.Categoryid,
            Count = Count,
            Unit = Unit,
            Provider = Provider,
            Cost = Cost,
            ImageName = ImageName,
            Description = Description
        },isValidObservable);
        
        AddImageCommand = ReactiveCommand.CreateFromTask(async () => await AddImage());
    }
    public ProductFormWindowViewModel(Product product)
    {
        Name = product.Name;
        Category = Helper.Database.Categories.Find(product.Categoryid)!;
        Count = product.Count; 
        Unit = product.Unit;
        Provider = product.Provider;
        Cost = product.Cost;
        ImageName = product.ImageName!;
        Description = product.Description!;
        _categories = new ObservableCollection<Category>(Helper.Database.Categories);
        
        var isValidObservable = this.WhenAnyValue(
            x => x.Name, 
            x => x.Category,
            x => x.Count,
            x => x.Unit,
            x => x.Provider,
            x => x.Cost,
            (name, category, count, unit, provider, cost) =>
                !string.IsNullOrEmpty(name) &&
                (category ?? _categories![0]) != null &&
                count >= 0 &&
                !string.IsNullOrEmpty(unit) &&
                !string.IsNullOrEmpty(provider) &&
                cost > 0);

        OkCommand = ReactiveCommand.Create(() => 
            {
                product.Name = Name;
                product.Categoryid = Category.Categoryid;
                product.Count = Count;
                product.Unit = Unit;
                product.Provider = Provider;
                product.Cost = Cost;
                product.ImageName = ImageName;
                product.Description = Description;
                return product;
            }, 
            isValidObservable);
        
        AddImageCommand = ReactiveCommand.CreateFromTask(async () => await AddImage());
    }
    public ReactiveCommand<Unit, Product> OkCommand { get; }
    public ICommand AddImageCommand { get; }
    public ObservableCollection<Category> Categories
    {
        get => _categories;
        set => this.RaiseAndSetIfChanged(ref _categories, value);
    }
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    public Category? Category
    {
        get => _category;
        set => this.RaiseAndSetIfChanged(ref _category, value);
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
    public string ImageName
    {
        get => _imageName!;
        set => this.RaiseAndSetIfChanged(ref _imageName, value);
    }
    public string Description
    {
        get => _description!;
        set => this.RaiseAndSetIfChanged(ref _description,value);
    }
    public Bitmap Image
    {
        get => _image!;
        set => this.RaiseAndSetIfChanged(ref _image, value);
    }
    private async Task AddImage()
    {
        var openFileDialog = new OpenFileDialog
        {
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "Image files aboba", Extensions = new List<string> { "jpg", "jpeg", "png" } }
            }
        };
        var result = await openFileDialog.ShowAsync(new Window());
        
        if (result?.Length > 0)
        {
            var filePath = result[0];
            var appDir = AppContext.BaseDirectory;
            var fileName = Path.GetFileName(filePath);
            var destinationPath = Path.Combine(appDir, "Assets", fileName);
            Directory.CreateDirectory(Path.Combine(appDir, "Assets"));
            if (ImageName != null) 
            {
                try
                {
                    File.Delete(Path.Combine(appDir, "Assets", ImageName));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            File.Copy(filePath, destinationPath, true);

            ImageName = fileName;
            Image = new Bitmap(Path.Combine(destinationPath));
        }
    }
}