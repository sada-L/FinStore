
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Avalonia.Media;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using Brushes = Avalonia.Media.Brushes;


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

    public string? ImageName { get; set; }

    public string? Description { get; set; }
    
    public virtual Category Category { get; set; } = null!;
    
    public virtual ICollection<Basket> Baskets { get; set; } = new List<Basket>();

    public Bitmap ProductImage => CompressImage(ImageName!);
    
    public bool IsAvailable => Count > 0;
    
    public IImmutableSolidColorBrush Color => IsAvailable ? Brushes.Transparent : Brushes.LightGray;
    
    private Bitmap CompressImage(string fileName)
    {
        var dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
        
        string filePath = fileName == null
            ? Path.Combine(dirPath, "picture.png")
            : File.Exists(Path.Combine(dirPath, ImageName!)) 
                ? Path.Combine(dirPath, ImageName!)
                : Path.Combine(dirPath, "picture.png") ;
        
        using (var originalImage = new System.Drawing.Bitmap(filePath))
        {
            var resizedImage = new System.Drawing.Bitmap(originalImage, new Size(200, 200));
            using (var stream = new MemoryStream())
            {
                resizedImage.Save(stream, ImageFormat.Jpeg);
                stream.Seek(0, SeekOrigin.Begin);
                return new Bitmap(stream);
            }
        }
    }
}
