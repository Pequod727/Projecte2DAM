using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class TaulaEstat
{
    public decimal Id { get; set; }

    public string Nom { get; set; } = null!;

    public string? Descripcio { get; set; }

    public string Imatge { get; set; } = null!;

    public virtual ICollection<CanviEstat> CanviEstats { get; set; } = new List<CanviEstat>();
}
