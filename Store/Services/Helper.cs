using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.Context;
using Store.Models;

namespace Store.Services;

public class Helper
{
    public static StoreContext Database = new StoreContext();
    
    public static async Task<List<Product>> GetAllProductsAsync()
    {
        return await Database.Products.ToListAsync();
    }

    public static async Task<Product> GetProductByIdAsync(int id)
    {
        return (await Database.Products.FindAsync(id))!;
    }

    public static async Task AddProductAsync(Product product)
    {
        Database.Products.Add(product);
        await Database.SaveChangesAsync();
        Database = new StoreContext();
    }

    public static async Task UpdateProductAsync(Product product)
    {
        Database.Products.Update(product);
        await Database.SaveChangesAsync();
        Database = new StoreContext();
    }

    public static async Task DeleteProductAsync(int id)
    {
        var product = await Database.Products.FindAsync(id);
        if (product != null)
        {
            Database.Products.Remove(product);
            await Database.SaveChangesAsync();
            Database = new StoreContext();
        }
    }

    public static async Task AddProductToBasketAsync(int id)
    {
        var basket = new Basket { Productid = id };
        Database.Baskets.Add(basket);
        await Database.SaveChangesAsync();
        Database = new StoreContext();
    }
}