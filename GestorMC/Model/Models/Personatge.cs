using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Personatge
{
    public int Id { get; set; } 

    public int Atac { get; set; }

    public int Defensa { get; set; } 

    public int Experiencia { get; set; } 

    public int Velocitat { get; set; } 

    public int Vida { get; set; } 

    public string Nom { get; set; } = null!;

    public string? Descripcio { get; set; }

    public string Imatge { get; set; } = null!;

    public string? Icona { get; set; }

    public bool Jugable { get; set; }

    public virtual ICollection<Habilitat> Habilitats { get; set; } = new List<Habilitat>();

    public virtual ICollection<Nivell> NivellIdEnemic1Navigations { get; set; } = new List<Nivell>();

    public virtual ICollection<Nivell> NivellIdEnemic2Navigations { get; set; } = new List<Nivell>();

    public virtual ICollection<Nivell> NivellIdEnemic3Navigations { get; set; } = new List<Nivell>();

    public virtual ICollection<Nivell> NivellIdEnemic4Navigations { get; set; } = new List<Nivell>();

    public virtual ICollection<PersonatgeItem> PersonatgeItems { get; set; } = new List<PersonatgeItem>();
}
