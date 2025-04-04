using System;
using System.Collections.Generic;

namespace DahuaSiteBootstrap.Model;

public partial class Notification
{
    public int Id { get; set; }

    public DateTime Sent { get; set; }

    public string Title { get; set; } = null!;

    public string? Message { get; set; }

    public int? Aid { get; set; }
}
