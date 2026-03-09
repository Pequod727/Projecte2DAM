using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Objectiu
{
    public decimal Id { get; set; }

    /// <summary>
    /// aliat, yo, enemic, aliats, enemics
    /// </summary>
    public string Nom { get; set; } = null!;

    public virtual ICollection<Efecte> Efectes { get; set; } = new List<Efecte>();
}
