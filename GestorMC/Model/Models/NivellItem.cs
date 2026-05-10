using System;

namespace Model.Models
{
    public partial class NivellItem
    {
        public int IdNivell { get; set; }
        public int IdItem { get; set; }

        public virtual Nivell IdNivellNavigation { get; set; } = null!;
        public virtual Item IdItemNavigation { get; set; } = null!;
    }
}