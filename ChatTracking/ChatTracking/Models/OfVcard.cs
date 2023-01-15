using System;
using System.Collections.Generic;

namespace ChatTracking.Models;

public partial class OfVcard
{
    public string Username { get; set; } = null!;

    public string Vcard { get; set; } = null!;
}
