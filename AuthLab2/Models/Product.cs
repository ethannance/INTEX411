using System;
using System.Collections.Generic;

namespace AuthLab2.Models;

public partial class Product
{
    public int product_ID { get; set; }

    public string name { get; set; } = null!;

    public int year { get; set; }

    public int num_parts { get; set; }

    public int price { get; set; }

    public string img_link { get; set; } = null!;

}
