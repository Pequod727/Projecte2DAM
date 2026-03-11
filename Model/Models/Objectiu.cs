using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Objectiu
{
    public decimal Id { get; set; }

    /// <summary>
    /// Tu, Enemic, Equip aliat, Equip enemic
    /// </summary>
    public string Nom { get; set; } = null!;

    public virtual ICollection<Efecte> Efectes { get; set; } = new List<Efecte>();
}
