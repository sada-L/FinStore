﻿using System;
using System.Collections.Generic;

namespace Store.Models;

public partial class Category
{
    public int Categoryid { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
