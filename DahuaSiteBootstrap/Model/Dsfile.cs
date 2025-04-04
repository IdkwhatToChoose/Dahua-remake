using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace DahuaSiteBootstrap.Model;

public partial class Dsfile
{
    public int Id { get; set; }

    public byte[] Data { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? DisplayName { get; set; }

    public string? Category { get; set; }

    public string? Content { get; set; }
}
public enum FileCategory
{
    Link_Software,
    Link_Manual,
    Powerpoint,
    Portfolio_item
}
public static class SelectCategory
{
    public static List<SelectListItem> Categories = new List<SelectListItem> { new SelectListItem { Disabled = true, Text = "Изберете категория", Value = "none", Selected = true } };

    static SelectCategory()
    {
        foreach (string category in Enum.GetNames(typeof(FileCategory)))
        {

            Categories.Add(new SelectListItem { Text = category.Replace("_", " "), Value = category.Replace("_", " ") });

        }
    }
}
