using System.Windows.Controls;

namespace Aplicacio.UserControls
{
    public partial class Slider : UserControl
    {
        public Slider() { InitializeComponent(); }

        public string Titol { get => lblTitol.Text; set => lblTitol.Text = value; }
        public double Valor { get => sldValor.Value; set => sldValor.Value = value; }
        public double Minim { get => sldValor.Minimum; set => sldValor.Minimum = value; }
        public double Maxim { get => sldValor.Maximum; set => sldValor.Maximum = value; }
    }
}