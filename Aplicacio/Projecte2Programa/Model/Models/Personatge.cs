using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Personatge
{
    public decimal Id { get; set; }

    public decimal Atac { get; set; }

    public decimal Defensa { get; set; }

    public decimal Experiencia { get; set; }

    public decimal Velocitat { get; set; }

    public decimal Vida { get; set; }

    public string Nom { get; set; } = null!;

    public string? Descripcio { get; set; }

    public byte[] Imatge { get; set; } = null!;

    public bool Jugable { get; set; }

    public virtual ICollection<Accio> Accios { get; set; } = new List<Accio>();
}
