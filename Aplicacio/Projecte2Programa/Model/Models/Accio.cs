using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Accio
{
    public decimal Id { get; set; }

    public decimal IdPersonatge { get; set; }

    public virtual ICollection<Efecte> Efectes { get; set; } = new List<Efecte>();

    public virtual Habilitat? Habilitat { get; set; }

    public virtual Personatge IdPersonatgeNavigation { get; set; } = null!;

    public virtual Item? Item { get; set; }
}
