using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Efecte
{
    public int Id { get; set; }

    public int IdAccio { get; set; }

    public string Nom { get; set; } = null!;

    public int? Probabilitat { get; set; }

    public int? IdEstat { get; set; }

    public int? IdEstadistica { get; set; }

    public int? Quantitat { get; set; }

    public int? Duracio { get; set; }

    public bool EsAfegir { get; set; }

    public virtual Accio IdAccioNavigation { get; set; } = null!;

    public virtual TaulaEstadistique? IdEstadisticaNavigation { get; set; }

    public virtual TaulaEstat? IdEstatNavigation { get; set; }
}
