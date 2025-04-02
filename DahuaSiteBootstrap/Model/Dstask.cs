using System;
using System.Collections.Generic;

namespace DahuaSiteBootstrap.Model;

public partial class Dstask
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Phone { get; set; } = null!;

    public int? AdminId { get; set; }

    public virtual Admin? Admin { get; set; }
}
