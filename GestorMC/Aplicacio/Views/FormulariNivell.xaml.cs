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
    // Aquesta és la classe de suport necessària per als ComboBox i les llistes visuals
    public class ComboItemModel
    {
        public int? Id { get; set; }
        public string Text { get; set; }
    }

    public partial class FormulariNivell : UserControl
    {
        private ModeFormulari _mode;
        private int? _idNivell;
        private Nivell _nivellActual;
        private ObservableCollection<ComboItemModel> _itemsSeleccionats = new ObservableCollection<ComboItemModel>();

        public FormulariNivell(ModeFormulari mode, int? idNivell = null)
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
                    // 1. Carregar Enemics (Filtrant els no jugables)
                    var enemics = db.Personatges
                        .Where(p => !p.Jugable)
                        .Select(p => new ComboItemModel { Id = p.Id, Text = $"{p.Id} - {p.Nom}" })
                        .ToList();

                    // Opció buida per si no es vol posar enemic en un slot
                    enemics.Insert(0, new ComboItemModel { Id = null, Text = "-- CAP --" });

                    cbEnemic1.ItemsSource = enemics.ToList(); cbEnemic1.DisplayMemberPath = "Text"; cbEnemic1.SelectedValuePath = "Id";
                    cbEnemic2.ItemsSource = enemics.ToList(); cbEnemic2.DisplayMemberPath = "Text"; cbEnemic2.SelectedValuePath = "Id";
                    cbEnemic3.ItemsSource = enemics.ToList(); cbEnemic3.DisplayMemberPath = "Text"; cbEnemic3.SelectedValuePath = "Id";
                    cbEnemic4.ItemsSource = enemics.ToList(); cbEnemic4.DisplayMemberPath = "Text"; cbEnemic4.SelectedValuePath = "Id";

                    // 2. Carregar Ítems disponibles per al Loot
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
                    _nivellActual = db.Nivells.FirstOrDefault(n => n.Id == _idNivell);

                    if (_nivellActual != null)
                    {
                        txtId.Text = _nivellActual.Id.ToString();
                        txtOrdre.Text = _nivellActual.Ordre.ToString();
                        txtFons.Text = _nivellActual.Fons;

                        cbEnemic1.SelectedValue = _nivellActual.IdEnemic1;
                        cbEnemic2.SelectedValue = _nivellActual.IdEnemic2;
                        cbEnemic3.SelectedValue = _nivellActual.IdEnemic3;
                        cbEnemic4.SelectedValue = _nivellActual.IdEnemic4;

                        // CARREGAR LOOT: Busquem explícitament a la taula NivellItem
                        var lootActual = db.NivellItems
                            .Include(ni => ni.IdItemNavigation)
                                .ThenInclude(i => i.IdAccioNavigation)
                            .Where(ni => ni.IdNivell == _idNivell)
                            .ToList();

                        _itemsSeleccionats.Clear();
                        foreach (var loot in lootActual)
                        {
                            // Omplim la interfície amb el que tenim guardat
                            _itemsSeleccionats.Add(new ComboItemModel
                            {
                                Id = loot.IdItem,
                                Text = loot.IdItemNavigation.IdAccioNavigation.Nom
                            });
                        }
                    }
                }
                else
                {
                    _nivellActual = new Nivell();
                    cbEnemic1.SelectedIndex = 0; cbEnemic2.SelectedIndex = 0;
                    cbEnemic3.SelectedIndex = 0; cbEnemic4.SelectedIndex = 0;
                    txtId.Text = "NOU";
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
                else
                {
                    imgFons.Source = null; txtPlatFons.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                imgFons.Source = null; txtPlatFons.Visibility = Visibility.Visible;
            }
        }

        private void BtnAfegirItem_Click(object sender, RoutedEventArgs e)
        {
            if (cbAfegirItem.SelectedItem is ComboItemModel seleccionat)
            {
                // Evitem duplicats a la llista de la UI
                if (!_itemsSeleccionats.Any(i => i.Id == seleccionat.Id))
                {
                    _itemsSeleccionats.Add(seleccionat);
                }
                cbAfegirItem.SelectedIndex = -1; // Buidem selecció
            }
        }

        private void BtnRemoureItem_Click(object sender, RoutedEventArgs e)
        {
            var boto = sender as Button;
            if (boto?.Tag != null)
            {
                var idItem = (int)boto.Tag;
                var itemParaEsborrar = _itemsSeleccionats.FirstOrDefault(x => x.Id == idItem);
                if (itemParaEsborrar != null) _itemsSeleccionats.Remove(itemParaEsborrar);
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // VALIDACIÓ 1: Segons el Trigger de SQL, el nivell no pot estar buit d'enemics
            if (cbEnemic1.SelectedValue == null &&
                cbEnemic2.SelectedValue == null &&
                cbEnemic3.SelectedValue == null &&
                cbEnemic4.SelectedValue == null)
            {
                MessageBox.Show("Has d'assignar com a mínim un enemic al nivell per poder-lo guardar.", "Dades Incompletes", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // VALIDACIÓ 2: Format de l'Ordre
            if (!int.TryParse(txtOrdre.Text, out int ordreParsed))
            {
                MessageBox.Show("El número d'Ordre ha de ser un valor numèric vàlid.", "Error de format", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    if (_mode == ModeFormulari.Edicio)
                    {
                        _nivellActual = db.Nivells.First(n => n.Id == _idNivell);
                    }

                    _nivellActual.Ordre = ordreParsed;
                    _nivellActual.Fons = txtFons.Text;

                    _nivellActual.IdEnemic1 = (int?)cbEnemic1.SelectedValue;
                    _nivellActual.IdEnemic2 = (int?)cbEnemic2.SelectedValue;
                    _nivellActual.IdEnemic3 = (int?)cbEnemic3.SelectedValue;
                    _nivellActual.IdEnemic4 = (int?)cbEnemic4.SelectedValue;

                    if (_mode == ModeFormulari.Creacio) db.Nivells.Add(_nivellActual);
                    else db.Update(_nivellActual);

                    // Guardem les dades principals del nivell (IMPORTANT: Crea l'ID si era un nivell nou)
                    db.SaveChanges();

                    // ==========================================
                    // GESTIÓ DEL LOOT (Taula intermèdia explícita)
                    // ==========================================

                    // 1. Esborrem tot el loot antic assignat a aquest nivell
                    var lootAntic = db.NivellItems.Where(ni => ni.IdNivell == _nivellActual.Id);
                    db.NivellItems.RemoveRange(lootAntic);

                    // 2. Inserim el loot que hi ha actualment a la llista visual
                    foreach (var itemUI in _itemsSeleccionats)
                    {
                        db.NivellItems.Add(new NivellItem
                        {
                            IdNivell = _nivellActual.Id,
                            IdItem = (int)itemUI.Id
                        });
                    }

                    // Guardem els canvis de la taula de loot
                    db.SaveChanges();

                    MessageBox.Show("Nivell guardat correctament!", "Èxit", MessageBoxButton.OK, MessageBoxImage.Information);
                    Tornar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar a la base de dades. És possible que l'ordre ja estigui duplicat.\n\nDetall: " + ex.Message, "Error de base de dades", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnTornar_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Perdre els canvis no guardats?", "Sortir", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Tornar();
            }
        }

        private void Tornar()
        {
            System.Windows.Navigation.NavigationService.GetNavigationService(this)?.Navigate(new VistaNivell());
        }
    }
}