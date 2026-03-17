using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Efecte
{
    public decimal Id { get; set; }

    public decimal IdAccio { get; set; }

    public string Nom { get; set; } = null!;

    public decimal? Probabilitat { get; set; }

    public decimal? IdEstat { get; set; }

    public decimal? IdEstadistica { get; set; }

    public decimal? Quantitat { get; set; }

    public decimal? Duracio { get; set; }

    public bool EsAfegir { get; set; }

    public virtual Accio IdAccioNavigation { get; set; } = null!;

    public virtual TaulaEstadistique? IdEstadisticaNavigation { get; set; }

    public virtual TaulaEstat? IdEstatNavigation { get; set; }
}
