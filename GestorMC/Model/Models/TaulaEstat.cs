using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class TaulaEstat
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;

    public string? Descripcio { get; set; }

    public string Icona { get; set; } = null!;

    public virtual ICollection<Efecte> Efectes { get; set; } = new List<Efecte>();
}
