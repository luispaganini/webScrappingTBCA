﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace WebScrapping_Foods.Entities;

public partial class UnitMeasurement
{
    public long Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<FoodNutritionInfo> FoodNutritionInfos { get; set; } = new List<FoodNutritionInfo>();
}