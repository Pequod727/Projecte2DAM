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

        // --- CÀRREGA I FILTRAT ---
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

        // --- ACCIONS DE LA LLISTA ---
        private void dgPersonatges_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Comprovem que s'hagi fet doble clic a una fila vàlida
            if (dgPersonatges.SelectedItem is Personatge personatgeSeleccionat)
            {
                // Obrim el formulari en Mode Vista passant-li l'ID del personatge
                NavigationService?.Navigate(new FormulariPersonatge(ModeFormulari.Vista, personatgeSeleccionat.Id));
            }
        }

        private void BtnEsborrar_Click(object sender, RoutedEventArgs e)
        {
            var boto = sender as Button;
            var personatge = boto?.DataContext as Personatge;

            if (personatge == null) return;

            MessageBoxResult resultat = MessageBox.Show(
                $"Estàs segur que vols esborrar el personatge '{personatge.Nom}' (Cod: {personatge.Id})? \n\nAquesta acció esborrarà també totes les seves habilitats/ítems associats.",
                "Confirmació d'esborrat",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultat == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        var persDB = db.Personatges
                                       .Include(p => p.Accios)
                                       .FirstOrDefault(p => p.Id == personatge.Id);

                        if (persDB != null)
                        {
                            if (persDB.Accios.Any())
                            {
                                db.Accios.RemoveRange(persDB.Accios);
                            }

                            db.Personatges.Remove(persDB);
                            db.SaveChanges();

                            CarregarDades(txtFiltre.Text.Trim());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No s'ha pogut esborrar. Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // --- NAVEGACIÓ ---
        private void BtnNou_Click(object sender, RoutedEventArgs e)
        {
            // Obrim el formulari en mode Creació (sense passar cap ID)
            NavigationService?.Navigate(new FormulariPersonatge(ModeFormulari.Creacio));
        }

        private void BtnTornar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Menu());
        }
    }
}