﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace WebScrapping_Foods.Models;

public partial class FoodGroup
{
    public long Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Food> Foods { get; set; } = new List<Food>();
}