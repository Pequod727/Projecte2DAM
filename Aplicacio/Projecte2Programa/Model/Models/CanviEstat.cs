using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class CanviEstat
{
    public decimal IdEfecte { get; set; }

    public decimal IdEstat { get; set; }

    public virtual Efecte IdEfecteNavigation { get; set; } = null!;

    public virtual TaulaEstat IdEstatNavigation { get; set; } = null!;
}
