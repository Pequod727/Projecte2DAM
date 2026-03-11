using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class CanviEstat
{
    public decimal IdEfecte { get; set; }

    public decimal IdEstat { get; set; }

    /// <summary>
    /// 0 és instantani
    /// </summary>
    public decimal? DuracioTorns { get; set; }

    /// <summary>
    /// TRUE per afegir estat, FALSE per curar
    /// </summary>
    public bool Aplicar { get; set; }

    public virtual Efecte IdEfecteNavigation { get; set; } = null!;

    public virtual TaulaEstat IdEstatNavigation { get; set; } = null!;
}
