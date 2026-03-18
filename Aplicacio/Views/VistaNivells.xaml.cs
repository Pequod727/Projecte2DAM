using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Model.Models;

namespace Aplicacio.Views
{
    public partial class VistaNivell : Page
    {
        public VistaNivell()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CarregarDades();
        }

        private void CarregarDades(string filtre = "")
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var query = db.Nivells.AsQueryable();

                    // El Nivell no té "Nom", així que filtrem per ID o per Ordre
                    if (!string.IsNullOrWhiteSpace(filtre))
                    {
                        if (decimal.TryParse(filtre, out decimal valorCerca))
                        {
                            query = query.Where(n => n.Id == valorCerca || n.Ordre == valorCerca);
                        }
                    }

                    dgNivells.ItemsSource = query.OrderBy(n => n.Ordre).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al carregar les dades: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnTornar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Menu());
        }

        private void txtFiltre_TextChanged(object sender, TextChangedEventArgs e)
        {
            CarregarDades(txtFiltre.Text);
        }

        private void BtnNou_Click(object sender, RoutedEventArgs e)
        {
            // Obrim el formulari en mode Creació (sense passar cap ID)
            NavigationService?.Navigate(new FormulariNivell(ModeFormulari.Creacio));
        }

        private void dgNivells_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgNivells.SelectedItem is Nivell nivellSeleccionat)
            {
                // Obrim el formulari en mode Edició passant-li la ID del nivell seleccionat
                NavigationService?.Navigate(new FormulariNivell(ModeFormulari.Edicio, nivellSeleccionat.Id));
            }
        }

        private void BtnEsborrar_Click(object sender, RoutedEventArgs e)
        {
            var boto = sender as Button;
            var nivellContext = boto?.DataContext as Nivell;

            if (nivellContext != null)
            {
                MessageBoxResult resultat = MessageBox.Show(
                    $"Estàs segur que vols esborrar el nivell {nivellContext.Ordre} (ID: {nivellContext.Id})?",
                    "Confirmació d'esborrat",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultat == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var db = new AppDbContext())
                        {
                            var nivell = db.Nivells.Find(nivellContext.Id);
                            if (nivell != null)
                            {
                                db.Nivells.Remove(nivell);
                                db.SaveChanges();
                                CarregarDades(txtFiltre.Text); // Refresca mantenint el filtre
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al esborrar el registre. \n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}