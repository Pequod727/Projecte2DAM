using System.Windows;
using System.Windows.Controls;

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
            if (boto != null)
            {
                if (boto.Content.ToString() == "Gestió de Personatges")
                {
                    NavigationService?.Navigate(new VistaPersonatges());
                }
                // Els altres botons els enllaçarem quan fem les vistes
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