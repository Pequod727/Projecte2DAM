using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Model.Models;

namespace Aplicacio.Views
{
    public partial class VistaPersonatges : Page
    {
        public VistaPersonatges()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CarregarDades();
        }

        // Càrrega amb filtre actualitzada
        private void CarregarDades(string filtre = "")
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var query = db.Personatges.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(filtre))
                    {
                        if (decimal.TryParse(filtre, out decimal idBuscado))
                        {
                            query = query.Where(p => p.Id == idBuscado || p.Nom.Contains(filtre));
                        }
                        else
                        {
                            query = query.Where(p => p.Nom.Contains(filtre));
                        }
                    }

                    dgPersonatges.ItemsSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al carregar les dades: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtFiltre_TextChanged(object sender, TextChangedEventArgs e)
        {
            CarregarDades(txtFiltre.Text.Trim());
        }

        private void dgPersonatges_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgPersonatges.SelectedItem is Personatge personatgeSeleccionat)
            {
                NavigationService?.Navigate(new FormulariPersonatge(ModeFormulari.Vista, personatgeSeleccionat.Id));
            }
        }

        private void BtnEsborrar_Click(object sender, RoutedEventArgs e)
        {
            var boto = sender as Button;
            var personatge = boto?.DataContext as Personatge;

            if (personatge == null) return;

            MessageBoxResult resultat = MessageBox.Show(
                $"Vols esborrar '{personatge.Nom}'? \n\nAixò eliminarà totes les seves habilitats associades.",
                "Confirmació d'esborrat",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultat == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        // Busquem el personatge. Gràcies al CASCADE del SQL, 
                        // en esborrar el personatge, les habilitats i accions cauen soles.
                        var persDB = db.Personatges.FirstOrDefault(p => p.Id == personatge.Id);

                        if (persDB != null)
                        {
                            db.Personatges.Remove(persDB);
                            db.SaveChanges();
                            CarregarDades(txtFiltre.Text.Trim());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No s'ha pogut esborrar: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnNou_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new FormulariPersonatge(ModeFormulari.Creacio));
        }

        private void BtnTornar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Menu());
        }
    }
}