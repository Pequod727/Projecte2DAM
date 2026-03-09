using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Efecte
{
    public decimal Id { get; set; }

    public string Nom { get; set; } = null!;

    public string? Descripcio { get; set; }

    public decimal IdAccio { get; set; }

    public decimal IdObjectiu { get; set; }

    public decimal? Probabilitat { get; set; }

    public virtual CanviEstat? CanviEstat { get; set; }

    public virtual Accio IdAccioNavigation { get; set; } = null!;

    public virtual Objectiu IdObjectiuNavigation { get; set; } = null!;

    public virtual Modificador? Modificador { get; set; }
}
