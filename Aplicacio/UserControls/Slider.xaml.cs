using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Aplicacio.UserControls
{
    public partial class Slider : UserControl
    {
        public Slider() { InitializeComponent(); }

        public string Titol { get => lblTitol.Text; set => lblTitol.Text = value; }
        public double Valor { get => sldValor.Value; set => sldValor.Value = value; }
        public double Minim { get => sldValor.Minimum; set => sldValor.Minimum = value; }
        public double Maxim { get => sldValor.Maximum; set => sldValor.Maximum = value; }

        private void TxtValor_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidarIActualitzar();
        }

        private void TxtValor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ValidarIActualitzar();
                Keyboard.ClearFocus();
            }
        }

        private void ValidarIActualitzar()
        {
            if (double.TryParse(txtValor.Text, out double nouValor))
            {
                if (nouValor > Maxim) nouValor = Maxim;
                if (nouValor < Minim) nouValor = Minim;

                sldValor.Value = nouValor;
            }

            txtValor.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
        }
    }
}