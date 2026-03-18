using Microsoft.EntityFrameworkCore;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aplicacio.Views
{
    /// <summary>
    /// Lógica de interacción para VistaHabilitats.xaml
    /// </summary>
    public partial class VistaHabilitats : Page
    {
        public VistaHabilitats()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) => CarregarDades();

        private void CarregarDades(string filtre = "")
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var query = db.Accios
                        .Include(a => a.IdObjectiuNavigation)
                        .Include(a => a.Item)
                        .Include(a => a.Efectes)
                        .AsQueryable();

                    if (!string.IsNullOrWhiteSpace(filtre))
                        query = query.Where(a => a.Nom.Contains(filtre));

                    // Creem una llista d'objectes anònims o una classe de suport per mostrar el "Tipus"
                    var llista = query.ToList().Select(a => new {
                        a.Id,
                        a.Nom,
                        a.IdObjectiuNavigation,
                        a.Efectes,
                        TipusText = a.Item != null ? "ÍTEM" : "HABILITAT",
                        ColorTipus = a.Item != null ? "#E2B13C" : "#4A90E2" // Groc per ítems, blau per habilitats
                    }).ToList();

                    dgAccions.ItemsSource = llista;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void TxtFiltre_TextChanged(object sender, TextChangedEventArgs e) => CarregarDades(txtFiltre.Text);

        private void BtnNou_Click(object sender, RoutedEventArgs e)
        {
            // Naveguem al formulari d'accions en mode Creació
            NavigationService?.Navigate(new FormulariAccio(ModeFormulari.Creacio));
        }

        private void dgAccions_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var seleccionat = dgAccions.SelectedItem;
            if (seleccionat != null)
            {
                // Obtenim l'ID mitjançant reflection o dynamic perquè és un objecte anònim
                decimal id = (decimal)seleccionat.GetType().GetProperty("Id").GetValue(seleccionat);
                NavigationService?.Navigate(new FormulariAccio(ModeFormulari.Edicio, id));
            }
        }

        private void BtnEsborrar_Click(object sender, RoutedEventArgs e)
        {
            var seleccionat = (sender as Button)?.DataContext;
            if (seleccionat == null) return;

            decimal id = (decimal)seleccionat.GetType().GetProperty("Id").GetValue(seleccionat);

            if (MessageBox.Show("Segur que vols esborrar l'acció?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var accio = db.Accios.Find(id);
                    if (accio != null)
                    {
                        db.Accios.Remove(accio);
                        db.SaveChanges();
                        CarregarDades();
                    }
                }
            }
        }

        private void BtnTornar_Click(object sender, RoutedEventArgs e) => NavigationService?.GoBack();
    }
}
