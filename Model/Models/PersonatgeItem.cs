using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class PersonatgeItem
{
    public decimal IdPersonatge { get; set; }

    public decimal IdItem { get; set; }

    public decimal? QuantitatStock { get; set; }

    public virtual Item IdItemNavigation { get; set; } = null!;

    public virtual Personatge IdPersonatgeNavigation { get; set; } = null!;
}
