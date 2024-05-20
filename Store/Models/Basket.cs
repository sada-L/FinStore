using System;
using System.Collections.Generic;

namespace Store.Models;

public partial class Basket
{
    public int Basketid { get; set; }

    public int Productid { get; set; }

    public virtual Product Product { get; set; } = null!;
}
