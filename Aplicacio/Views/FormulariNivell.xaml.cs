using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;
using Model.Models;

namespace Aplicacio.Views
{
    // Classe de suport per als ComboBox i la llista d'ítems
    public class ComboItemModel
    {
        public decimal? Id { get; set; }
        public string Text { get; set; }
    }

    public partial class FormulariNivell : UserControl
    {
        private ModeFormulari _mode;
        private decimal? _idNivell;
        private Nivell _nivellActual;
        private ObservableCollection<ComboItemModel> _itemsSeleccionats = new ObservableCollection<ComboItemModel>();

        // IMPORTANT: Ara el constructor demana el mode i la ID opcional!
        public FormulariNivell(ModeFormulari mode, decimal? idNivell = null)
        {
            InitializeComponent();
            _mode = mode;
            _idNivell = idNivell;

            icItemsSeleccionats.ItemsSource = _itemsSeleccionats;

            CarregarDesplegables();
            CarregarDades();
        }

        private void CarregarDesplegables()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    // 1. Carregar Enemics (Personatges on Jugable == false)
                    var enemics = db.Personatges
                        .Where(p => !p.Jugable)
                        .Select(p => new ComboItemModel { Id = p.Id, Text = $"{p.Id} - {p.Nom}" })
                        .ToList();

                    // Afegim l'opció buida per si no volen posar cap enemic
                    enemics.Insert(0, new ComboItemModel { Id = null, Text = "-- CAP --" });

                    cbEnemic1.ItemsSource = enemics.ToList(); cbEnemic1.DisplayMemberPath = "Text"; cbEnemic1.SelectedValuePath = "Id";
                    cbEnemic2.ItemsSource = enemics.ToList(); cbEnemic2.DisplayMemberPath = "Text"; cbEnemic2.SelectedValuePath = "Id";
                    cbEnemic3.ItemsSource = enemics.ToList(); cbEnemic3.DisplayMemberPath = "Text"; cbEnemic3.SelectedValuePath = "Id";
                    cbEnemic4.ItemsSource = enemics.ToList(); cbEnemic4.DisplayMemberPath = "Text"; cbEnemic4.SelectedValuePath = "Id";

                    // 2. Carregar Ítems (Llegint la taula Acció per tenir el nom real de l'ítem)
                    var llistaItems = db.Items
                        .Include(i => i.IdAccioNavigation)
                        .Select(i => new ComboItemModel { Id = i.IdAccio, Text = i.IdAccioNavigation.Nom })
                        .ToList();

                    cbAfegirItem.ItemsSource = llistaItems;
                    cbAfegirItem.DisplayMemberPath = "Text";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al carregar les llistes: " + ex.Message);
            }
        }

        private void CarregarDades()
        {
            using (var db = new AppDbContext())
            {
                if (_mode == ModeFormulari.Edicio && _idNivell.HasValue)
                {
                    // Obtenim el nivell i fem Include als Ítems i la seva Accio per tenir els noms
                    _nivellActual = db.Nivells
                        .Include(n => n.IdItems)
                            .ThenInclude(i => i.IdAccioNavigation)
                        .FirstOrDefault(n => n.Id == _idNivell);

                    if (_nivellActual != null)
                    {
                        txtId.Text = _nivellActual.Id.ToString();
                        txtOrdre.Text = _nivellActual.Ordre.ToString();
                        txtFons.Text = _nivellActual.Fons;

                        cbEnemic1.SelectedValue = _nivellActual.IdEnemic1;
                        cbEnemic2.SelectedValue = _nivellActual.IdEnemic2;
                        cbEnemic3.SelectedValue = _nivellActual.IdEnemic3;
                        cbEnemic4.SelectedValue = _nivellActual.IdEnemic4;

                        // Omplim la UI amb els ítems guardats
                        foreach (var item in _nivellActual.IdItems)
                        {
                            _itemsSeleccionats.Add(new ComboItemModel { Id = item.IdAccio, Text = item.IdAccioNavigation.Nom });
                        }
                    }
                }
                else
                {
                    _nivellActual = new Nivell();
                    cbEnemic1.SelectedIndex = 0; cbEnemic2.SelectedIndex = 0;
                    cbEnemic3.SelectedIndex = 0; cbEnemic4.SelectedIndex = 0;
                }
            }
        }

        private void TxtFons_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtFons.Text) && Uri.IsWellFormedUriString(txtFons.Text, UriKind.Absolute))
                {
                    imgFons.Source = new BitmapImage(new Uri(txtFons.Text));
                    txtPlatFons.Visibility = Visibility.Collapsed;
                }
                else { imgFons.Source = null; txtPlatFons.Visibility = Visibility.Visible; }
            }
            catch { imgFons.Source = null; txtPlatFons.Visibility = Visibility.Visible; }
        }

        private void BtnAfegirItem_Click(object sender, RoutedEventArgs e)
        {
            if (cbAfegirItem.SelectedItem is ComboItemModel seleccionat)
            {
                // Evitem que s'afegeixi el mateix ítem dos cops
                if (!_itemsSeleccionats.Any(i => i.Id == seleccionat.Id))
                {
                    _itemsSeleccionats.Add(seleccionat);
                }
                cbAfegirItem.SelectedIndex = -1; // Netegem la selecció
            }
        }

        private void BtnRemoureItem_Click(object sender, RoutedEventArgs e)
        {
            var boto = sender as Button;
            if (boto?.Tag != null)
            {
                var idItem = (decimal)boto.Tag;
                var itemParaEsborrar = _itemsSeleccionats.FirstOrDefault(x => x.Id == idItem);
                if (itemParaEsborrar != null) _itemsSeleccionats.Remove(itemParaEsborrar);
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validació bàsica
            if (!decimal.TryParse(txtOrdre.Text, out decimal ordreParsed))
            {
                MessageBox.Show("El número d'Ordre ha de ser un valor numèric vàlid.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    // Si estem editant, hem de carregar l'entitat amb els seus items per poder netejar-los i refer-los
                    if (_mode == ModeFormulari.Edicio)
                    {
                        _nivellActual = db.Nivells.Include(n => n.IdItems).First(n => n.Id == _idNivell);
                    }

                    _nivellActual.Ordre = ordreParsed;
                    _nivellActual.Fons = txtFons.Text;

                    _nivellActual.IdEnemic1 = (decimal?)cbEnemic1.SelectedValue;
                    _nivellActual.IdEnemic2 = (decimal?)cbEnemic2.SelectedValue;
                    _nivellActual.IdEnemic3 = (decimal?)cbEnemic3.SelectedValue;
                    _nivellActual.IdEnemic4 = (decimal?)cbEnemic4.SelectedValue;

                    // Gestionem la relació Many-to-Many d'ítems
                    _nivellActual.IdItems.Clear(); // Buidem la llista actual

                    foreach (var itemUI in _itemsSeleccionats)
                    {
                        var dbItem = db.Items.Find(itemUI.Id);
                        if (dbItem != null) _nivellActual.IdItems.Add(dbItem);
                    }

                    if (_mode == ModeFormulari.Creacio) db.Nivells.Add(_nivellActual);
                    else db.Update(_nivellActual);

                    db.SaveChanges();

                    MessageBox.Show("Nivell guardat correctament!", "Èxit", MessageBoxButton.OK, MessageBoxImage.Information);
                    System.Windows.Navigation.NavigationService.GetNavigationService(this)?.Navigate(new VistaNivell());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar a la base de dades. És possible que el número d'Ordre ja estigui en ús per un altre nivell.\n\nDetall: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnTornar_Click(object sender, RoutedEventArgs e)
        {
            var resultat = MessageBox.Show("Segur que vols tornar? Es perdran els canvis no guardats.", "Confirmació", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (resultat == MessageBoxResult.Yes)
            {
                System.Windows.Navigation.NavigationService.GetNavigationService(this)?.Navigate(new VistaNivell());
            }
        }
    }
}