using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Modificador
{
    public decimal IdEfecte { get; set; }

    /// <summary>
    /// si es zero es instantani
    /// </summary>
    public decimal? DuracioTorns { get; set; }

    /// <summary>
    /// nom exacte de la estadistica
    /// </summary>
    public string Estadistica { get; set; } = null!;

    public decimal Quantitat { get; set; }

    public virtual Efecte IdEfecteNavigation { get; set; } = null!;
}
