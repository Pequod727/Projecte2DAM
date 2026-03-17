using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Item
{
    public decimal IdAccio { get; set; }

    public virtual Accio IdAccioNavigation { get; set; } = null!;

    public virtual ICollection<PersonatgeItem> PersonatgeItems { get; set; } = new List<PersonatgeItem>();

    public virtual ICollection<Nivell> IdNivells { get; set; } = new List<Nivell>();
}
