using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class TaulaEstadistique
{
    public decimal Id { get; set; }

    public string Nom { get; set; } = null!;

    public string Icona { get; set; } = null!;

    public virtual ICollection<Efecte> Efectes { get; set; } = new List<Efecte>();
}
