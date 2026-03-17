using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Habilitat
{
    public decimal IdAccio { get; set; }

    public decimal IdPersonatge { get; set; }

    public virtual Accio IdAccioNavigation { get; set; } = null!;

    public virtual Personatge IdPersonatgeNavigation { get; set; } = null!;
}
