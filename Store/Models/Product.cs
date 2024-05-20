
using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Store.Models;

public partial class Product
{
    public int Productid { get; set; }

    public string Name { get; set; } = null!;

    public int? Categoryid { get; set; }

    public int? Count { get; set; }

    public string Unit { get; set; } = null!;

    public string Provider { get; set; } = null!;

    public double? Cost { get; set; }

    public string? Imagepath { get; set; }

    public string? Description { get; set; }
    
    public virtual Category Category { get; set; } = null!;
    
    public virtual ICollection<Basket> Baskets { get; set; } = new List<Basket>();

    public Bitmap ProductImage => Imagepath == null
        ? new Bitmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/picture.png"))
        : File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Assets/{Imagepath}")) 
            ? new Bitmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Assets/{Imagepath}"))
            : new Bitmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/picture.png")) ;
    
    public bool IsAvailable => Count > 0;
    
    public IImmutableSolidColorBrush Color => IsAvailable ? Brushes.Transparent : Brushes.LightGray;
}
