using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Nivell
{
    public decimal Id { get; set; }

    /// <summary>
    /// Ordre d&apos;aparició (max 99)
    /// </summary>
    public decimal Ordre { get; set; }

    public decimal? IdEnemic1 { get; set; }

    public decimal? IdEnemic2 { get; set; }

    public decimal? IdEnemic3 { get; set; }

    public decimal? IdEnemic4 { get; set; }

    public virtual Personatge? IdEnemic1Navigation { get; set; }

    public virtual Personatge? IdEnemic2Navigation { get; set; }

    public virtual Personatge? IdEnemic3Navigation { get; set; }

    public virtual Personatge? IdEnemic4Navigation { get; set; }

    public virtual ICollection<Item> IdItems { get; set; } = new List<Item>();
}
