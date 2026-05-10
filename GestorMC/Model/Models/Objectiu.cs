using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Objectiu
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;

    public virtual ICollection<Accio> Accios { get; set; } = new List<Accio>();
}
