using System;
using System.Collections.Generic;

namespace DahuaSiteBootstrap.Model;

public partial class Client
{
    public int Id { get; set; }

    public string RecieverName { get; set; } = null!;

    public string RecieverCity { get; set; } = null!;

    public string RecieverAddress { get; set; } = null!;

    public string RecieverEik { get; set; } = null!;

    public string RintroughDds { get; set; } = null!;
}
