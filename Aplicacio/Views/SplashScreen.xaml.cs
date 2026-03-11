using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Aplicacio.Views
{
    public partial class SplashScreen : Page
    {
        public SplashScreen()
        {
            InitializeComponent();
            Loaded += SplashScreen_Loaded;
        }

        private async void SplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(2500);
            NavigationService?.Navigate(new Menu());
        }
    }
}