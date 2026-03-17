using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using Aplicacio.UserControls;

namespace Aplicacio.Views
{
    // ENUM I CLASSE DE SUPORT PÚBLICS
    public enum ModeFormulari { Creacio, Edicio, Vista }

    public class AccioComboItem
    {
        public decimal Id { get; set; }
        public string Nom { get; set; }
        public string TextAMostrar => $"{Id} - {Nom}";
    }

    public partial class FormulariPersonatge : UserControl
    {
        private ModeFormulari _mode;
        private decimal? _idPersonatge;
        private Personatge _personatgeActual;
        private List<AccioComboItem> _totesLesAccions;
        private ObservableCollection<AccioComboItem> _habilitatsSeleccionades = new ObservableCollection<AccioComboItem>();

        public FormulariPersonatge(ModeFormulari mode, decimal? idPersonatge = null)
        {
            InitializeComponent();
            _mode = mode;
            _idPersonatge = idPersonatge;

            icHabilitatsSeleccionades.ItemsSource = _habilitatsSeleccionades;
            InicialitzarHabilitats();
            CarregarDades();
        }

        private void InicialitzarHabilitats()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    _totesLesAccions = db.Accios.Select(a => new AccioComboItem { Id = a.Id, Nom = a.Nom }).ToList();
                    RefrescarCombo();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error DB: " + ex.Message); }
        }

        private void RefrescarCombo()
        {
            if (_totesLesAccions == null) return;
            cbAfegirHabilitat.ItemsSource = _totesLesAccions
                .Where(a => !_habilitatsSeleccionades.Any(s => s.Id == a.Id))
                .ToList();
            cbAfegirHabilitat.DisplayMemberPath = "TextAMostrar";
        }

        private void CbAfegirHabilitat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbAfegirHabilitat.SelectedItem is AccioComboItem seleccionada)
            {
                if (_habilitatsSeleccionades.Count < 4)
                {
                    _habilitatsSeleccionades.Add(seleccionada);
                    RefrescarCombo();
                }
                else MessageBox.Show("Màxim 4 habilitats permeses.");

                cbAfegirHabilitat.SelectedIndex = -1;
            }
        }

        private void BtnRemoureHabilitat_Click(object sender, RoutedEventArgs e)
        {
            var boto = sender as Button;
            if (boto?.Tag != null)
            {
                var id = (decimal)boto.Tag;
                var item = _habilitatsSeleccionades.FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    _habilitatsSeleccionades.Remove(item);
                    RefrescarCombo();
                }
            }
        }

        private void CarregarDades()
        {
            using (var db = new AppDbContext())
            {
                if (_idPersonatge.HasValue)
                {
                    _personatgeActual = db.Personatges.Include(p => p.Habilitats).FirstOrDefault(p => p.Id == _idPersonatge);
                    if (_personatgeActual != null)
                    {
                        txtId.Text = _personatgeActual.Id.ToString();
                        txtNom.Text = _personatgeActual.Nom;
                        txtDescripcio.Text = _personatgeActual.Descripcio;
                        txtImatge.Text = _personatgeActual.Imatge;
                        txtIcona.Text = _personatgeActual.Icona;
                        if (_personatgeActual.Jugable) rbPersonatge.IsChecked = true; else rbEnemic.IsChecked = true;

                        sldVida.Valor = (double)_personatgeActual.Vida;
                        sldAtac.Valor = (double)_personatgeActual.Atac;
                        sldDefensa.Valor = (double)_personatgeActual.Defensa;
                        sldVelocitat.Valor = (double)_personatgeActual.Velocitat;
                        sldExperiencia.Valor = (double)_personatgeActual.Experiencia;

                        foreach (var h in _personatgeActual.Habilitats)
                        {
                            var item = _totesLesAccions.FirstOrDefault(x => x.Id == h.IdAccio);
                            if (item != null) _habilitatsSeleccionades.Add(item);
                        }
                        RefrescarCombo();
                    }
                }
                else _personatgeActual = new Personatge();
            }
        }

        private void TxtImatge_TextChanged(object sender, TextChangedEventArgs e) => ActualitzarPreview(txtImatge.Text, imgPrev, txtPlatImg);
        private void TxtIcona_TextChanged(object sender, TextChangedEventArgs e) => ActualitzarPreview(txtIcona.Text, icoPrev, txtPlatIco);

        private void ActualitzarPreview(string url, Image img, TextBlock placeholder)
        {
            try
            {
                if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    img.Source = new BitmapImage(new Uri(url));
                    placeholder.Visibility = Visibility.Collapsed;
                }
                else { img.Source = null; placeholder.Visibility = Visibility.Visible; }
            }
            catch { img.Source = null; placeholder.Visibility = Visibility.Visible; }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // 1. VALIDACIONS PRÈVIES
            if (string.IsNullOrWhiteSpace(txtNom.Text))
            {
                MessageBox.Show("El nom del personatge és obligatori.", "Dades incompletes", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Tallem l'execució aquí
            }

            if (string.IsNullOrWhiteSpace(txtImatge.Text))
            {
                MessageBox.Show("La URL de la imatge principal és obligatòria.", "Dades incompletes", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_habilitatsSeleccionades.Count == 0)
            {
                MessageBox.Show("Un personatge no pot tenir menys d'1 habilitat. Siusplau, afegeix-ne alguna al catàleg.", "Error de validació", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. GUARDAR DADES (Si passa les validacions)
            try
            {
                using (var db = new AppDbContext())
                {
                    _personatgeActual.Nom = txtNom.Text;
                    _personatgeActual.Descripcio = txtDescripcio.Text;
                    _personatgeActual.Imatge = txtImatge.Text;
                    _personatgeActual.Icona = string.IsNullOrWhiteSpace(txtIcona.Text) ? null : txtIcona.Text;
                    _personatgeActual.Jugable = rbPersonatge.IsChecked ?? true;
                    _personatgeActual.Vida = (decimal)sldVida.Valor;
                    _personatgeActual.Atac = (decimal)sldAtac.Valor;
                    _personatgeActual.Defensa = (decimal)sldDefensa.Valor;
                    _personatgeActual.Velocitat = (decimal)sldVelocitat.Valor;
                    _personatgeActual.Experiencia = (decimal)sldExperiencia.Valor;

                    if (_mode == ModeFormulari.Creacio)
                        db.Personatges.Add(_personatgeActual);
                    else
                        db.Update(_personatgeActual);

                    db.SaveChanges();

                    // Gestionem les habilitats (esborrem les velles i afegim les noves)
                    var velles = db.Habilitats.Where(h => h.IdPersonatge == _personatgeActual.Id);
                    db.Habilitats.RemoveRange(velles);
                    foreach (var h in _habilitatsSeleccionades)
                    {
                        db.Habilitats.Add(new Habilitat { IdPersonatge = _personatgeActual.Id, IdAccio = h.Id });
                    }

                    db.SaveChanges();
                    MessageBox.Show("Les dades s'han guardat correctament!", "Èxit", MessageBoxButton.OK, MessageBoxImage.Information);
                    System.Windows.Navigation.NavigationService.GetNavigationService(this)?.Navigate(new VistaPersonatges());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("S'ha produït un error al guardar: " + ex.Message, "Error Crític", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnTornar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resultat = MessageBox.Show(
                "Estàs segur que vols cancel·lar? Es perdran totes les dades no guardades.",
                "Confirmació de sortida",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultat == MessageBoxResult.Yes)
            {
                System.Windows.Navigation.NavigationService.GetNavigationService(this)?.Navigate(new VistaPersonatges());
            }
        }
    }
}