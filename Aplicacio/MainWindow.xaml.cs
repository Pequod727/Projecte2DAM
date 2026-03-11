using System.Windows;

namespace Aplicacio
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Views.SplashScreen());
        }
    }
}