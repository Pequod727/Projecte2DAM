using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Aplicacio.Views
{
    public partial class Menu : Page
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void BtnOpcio_Click(object sender, RoutedEventArgs e)
        {
            var boto = sender as Button;

            // Verifiquem que el botó i el Tag no siguin nuls per evitar excepcions
            if (boto != null && boto.Tag != null)
            {
                string opcio = boto.Tag.ToString();

                // Naveguem segons el valor del Tag definit al XAML
                switch (opcio)
                {
                    case "Personatges":
                        NavigationService?.Navigate(new VistaPersonatges());
                        break;
                    case "Nivells":
                        // Aquí aniria la navegació a Nivells quan estigui llesta
                        break;
                    case "Habilitats":
                        // Aquí aniria la navegació a Habilitats quan estigui llesta
                        break;
                }
            }
        }

        private void BtnSortir_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resultat = MessageBox.Show(
                "Estàs segur que vols sortir del Gestor de Moment Crític?",
                "Confirmació",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultat == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}