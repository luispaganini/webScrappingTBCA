﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace WebScrapping_Foods.Models;

public partial class NutritionalComposition
{
    public long Id { get; set; }

    public string Item { get; set; }

    public virtual ICollection<FoodNutritionInfo> FoodNutritionInfos { get; set; } = new List<FoodNutritionInfo>();
}