using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Item
{
    public decimal IdAccio { get; set; }

    public virtual Accio IdAccioNavigation { get; set; } = null!;
}
