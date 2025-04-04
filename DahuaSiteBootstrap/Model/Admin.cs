using System;
using System.Collections.Generic;

namespace DahuaSiteBootstrap.Model;

public partial class Admin
{
    public int Id { get; set; }

    public string AdminName { get; set; } = null!;

    public string? Password { get; set; }

    public string? Type { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Dstask> Dstasks { get; } = new List<Dstask>();
}
