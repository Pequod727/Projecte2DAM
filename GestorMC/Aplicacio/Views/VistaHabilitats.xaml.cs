using Microsoft.EntityFrameworkCore;
using Model.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Aplicacio.Views
{
    public partial class VistaHabilitats : Page
    {
        public VistaHabilitats()
        {
            InitializeComponent();
            // Assignem l'esdeveniment també aquí per doble seguretat
            this.Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) => CarregarDades();

        private void CarregarDades(string filtre = "")
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    // 1. Chivato per saber si la base de dades s'està connectant bé
                    int totalAccionsBD = db.Accios.Count();
                    if (totalAccionsBD == 0)
                    {
                        // Si està literalment buida, avisem amablement.
                        dgAccions.ItemsSource = null;
                        return;
                    }

                    // 2. Carreguem les accions a memòria SENSE fer Include d'Objectiu per evitar que 
                    // els Inner Joins d'EF Core ens amaguin dades si hi ha inconsistències
                    var accions = db.Accios
                        .Include(a => a.Item)
                        .Include(a => a.Efectes)
                        .ToList();

                    // Carreguem els objectius a banda per poder creuar les dades manualment
                    var objectius = db.Objectius.ToList();

                    // Filtrem si hi ha text al buscador
                    if (!string.IsNullOrWhiteSpace(filtre))
                    {
                        accions = accions.Where(a => a.Nom.ToLower().Contains(filtre.ToLower()) || a.Id.ToString() == filtre).ToList();
                    }

                    // 3. Muntem la llista per a la graella visual
                    var llista = accions.Select(a => new {
                        Id = a.Id,
                        Nom = a.Nom,
                        TipusText = a.Item != null ? "ÍTEM" : "HABILITAT",
                        ColorTipus = a.Item != null ? "#E2B13C" : "#4A90E2",

                        // Busquem el nom de l'objectiu creuant les llistes de memòria.
                        // Si no el troba, mostrem la ID perquè vegis quin objectiu està fallant a la BD.
                        NomObjectiu = objectius.FirstOrDefault(o => o.Id == a.IdObjectiu)?.Nom ?? $"ERROR OBJ (ID: {a.IdObjectiu})",

                        TotalEfectes = a.Efectes?.Count ?? 0
                    }).ToList();

                    dgAccions.ItemsSource = llista;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error greu de connexió amb la Base de Dades:\n{ex.Message}\n\nDetall:\n{ex.InnerException?.Message}", "Error crític", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtFiltre_TextChanged(object sender, TextChangedEventArgs e) => CarregarDades(txtFiltre.Text);

        private void BtnNou_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new FormulariAccio(ModeFormulari.Creacio));
        }

        private void dgAccions_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var seleccionat = dgAccions.SelectedItem;
            if (seleccionat != null)
            {
                // Extraiem la ID dinàmicament de l'objecte anònim
                int id = (int)seleccionat.GetType().GetProperty("Id").GetValue(seleccionat);
                NavigationService?.Navigate(new FormulariAccio(ModeFormulari.Edicio, id));
            }
        }

        private void BtnEsborrar_Click(object sender, RoutedEventArgs e)
        {
            var seleccionat = (sender as Button)?.DataContext;
            if (seleccionat == null) return;

            int id = (int)seleccionat.GetType().GetProperty("Id").GetValue(seleccionat);

            if (MessageBox.Show("Segur que vols esborrar l'acció?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var accio = db.Accios.Find(id);
                    if (accio != null)
                    {
                        db.Accios.Remove(accio);
                        db.SaveChanges();
                        CarregarDades(txtFiltre.Text); // Refresquem la llista aplicant el filtre actual
                    }
                }
            }
        }

        private void BtnTornar_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new Menu());
    }
}