using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class PersonatgeItem
{
    public int IdPersonatge { get; set; }

    public int IdItem { get; set; }

    public int? QuantitatStock { get; set; }

    public virtual Item IdItemNavigation { get; set; } = null!;

    public virtual Personatge IdPersonatgeNavigation { get; set; } = null!;
}