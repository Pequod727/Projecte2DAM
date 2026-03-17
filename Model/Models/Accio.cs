using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Accio
{
    public decimal Id { get; set; }

    public decimal IdObjectiu { get; set; }

    public string Nom { get; set; } = null!;

    public string? Descripcio { get; set; }

    public string Icona { get; set; } = null!;

    public virtual ICollection<Efecte> Efectes { get; set; } = new List<Efecte>();

    public virtual Habilitat? Habilitat { get; set; }

    public virtual Objectiu IdObjectiuNavigation { get; set; } = null!;

    public virtual Item? Item { get; set; }
}
