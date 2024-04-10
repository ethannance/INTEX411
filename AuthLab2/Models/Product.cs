using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthLab2.Models;

public partial class Product
{
    [Key]
    public int row_ID { get; set; }
    public int product_ID { get; set; }
    [Required(ErrorMessage = "Please include a product name")]
    public string name { get; set; } = null!;
    [Required(ErrorMessage = "Please include a release year")]
    public int year { get; set; }
    [Required(ErrorMessage = "Please include the number of parts")]
    public int num_parts { get; set; }
    [Required(ErrorMessage = "Please include a price")]
    public int price { get; set; }
    [Required(ErrorMessage = "Please include an image link")]
    public string img_link { get; set; } = null!;
    [Required(ErrorMessage = "Please include a primary colors")]
    public string primary_color { get; set; }
    [Required(ErrorMessage = "Please include a secondary color")]
    public string secondary_color { get; set; }
    [Required(ErrorMessage = "Please include a description")]
    public string description { get; set; }
    [Required(ErrorMessage = "Please include a category")]
    public string category {  get; set; }

}
