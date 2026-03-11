using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Modificador
{
    public decimal IdEfecte { get; set; }

    public decimal? DuracioTorns { get; set; }

    public string Estadistica { get; set; } = null!;

    public decimal Quantitat { get; set; }

    public virtual Efecte IdEfecteNavigation { get; set; } = null!;
}
