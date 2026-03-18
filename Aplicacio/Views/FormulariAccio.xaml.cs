using Microsoft.EntityFrameworkCore;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Lógica de interacción para FormulariAccio.xaml
    /// </summary>
    public partial class FormulariAccio : UserControl
    {
        private ModeFormulari _mode;
        private decimal? _idAccio;
        private Accio _accioActual;

        // Llista d'efectes per al ListBox
        private ObservableCollection<Efecte> _efectesLlista = new ObservableCollection<Efecte>();
        private Efecte _efecteEnEdicio;

        public FormulariAccio(ModeFormulari mode, decimal? idAccio = null)
        {
            InitializeComponent();
            _mode = mode;
            _idAccio = idAccio;

            lbEfectes.ItemsSource = _efectesLlista;

            InicialitzarCombos();
            CarregarDades();
        }

        private void InicialitzarCombos()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    // Carreguem els objectius (Aliat, Enemic, Jo...)
                    cbObjectiu.ItemsSource = db.Objectius.ToList();
                    cbObjectiu.DisplayMemberPath = "Nom";
                    cbObjectiu.SelectedValuePath = "Id";

                    // Carreguem les estadístiques (Vida, Atac...)
                    cbEstadistica.ItemsSource = db.TaulaEstadistiques.ToList();
                    cbEstadistica.DisplayMemberPath = "Nom";
                    cbEstadistica.SelectedValuePath = "Id";

                    // Carreguem els estats (Cregat, Atordit...)
                    cbEstats.ItemsSource = db.TaulaEstats.ToList();
                    cbEstats.DisplayMemberPath = "Nom";
                    cbEstats.SelectedValuePath = "Id";
                }
            }
            catch (Exception ex) { MessageBox.Show("Error carregant catàlegs: " + ex.Message); }
        }

        private void CarregarDades()
        {
            using (var db = new AppDbContext())
            {
                if (_idAccio.HasValue)
                {
                    _accioActual = db.Accios.Include(a => a.Efectes).FirstOrDefault(a => a.Id == _idAccio);
                    if (_accioActual != null)
                    {
                        txtIdAccio.Text = _accioActual.Id.ToString();
                        txtNomAccio.Text = _accioActual.Nom;

                        // Determinem si és Habilitat o Item (simplificació segons el model)
                        rbHabilitat.IsChecked = _accioActual.Habilitat != null;
                        rbItem.IsChecked = _accioActual.Item != null;

                        foreach (var e in _accioActual.Efectes) _efectesLlista.Add(e);
                    }
                }
                else
                {
                    _accioActual = new Accio();
                    txtIdAccio.Text = "NOVA";
                }
            }
        }

        #region GESTIÓ D'EFECTES (COLUMNA 1)

        private void BtnAfegirEfecte_Click(object sender, RoutedEventArgs e)
        {
            if (_efectesLlista.Count >= 20)
            {
                MessageBox.Show("Has arribat al màxim de 20 efectes.");
                return;
            }

            var nouEfecte = new Efecte { Nom = "Nou Efecte", Probabilitat = 100, Quantitat = 0, Duracio = 0 };
            _efectesLlista.Add(nouEfecte);
            PrepararEdicioEfecte(nouEfecte);
        }

        private void LbEfectes_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lbEfectes.SelectedItem is Efecte seleccionat)
            {
                PrepararEdicioEfecte(seleccionat);
            }
        }

        private void BtnEliminarEfecte_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var ef = btn?.DataContext as Efecte;
            if (ef != null)
            {
                _efectesLlista.Remove(ef);
                if (_efecteEnEdicio == ef) DesactivarEdicio();
            }
        }

        #endregion

        #region CONFIGURACIÓ EFECTE (COLUMNA 2 i 3)

        private void PrepararEdicioEfecte(Efecte ef)
        {
            _efecteEnEdicio = ef;
            panelConfigEfecte.IsEnabled = true;
            panelDetallsEspecífics.IsEnabled = true;

            // Carregar dades al panell central
            txtNomEfecte.Text = ef.Nom;
            cbObjectiu.SelectedValue = ef.IdAccio; // O la FK corresponent
            sldProbabilitat.Valor = (double)(ef.Probabilitat ?? 100);

            // Determinar tipus d'efecte per mostrar Columna 3
            if (ef.IdEstat != null) rbCanviEstat.IsChecked = true;
            else rbModEstadistica.IsChecked = true;

            ActualitzarInterficieEspecífica();

            // Carregar dades columna 3
            cbEstadistica.SelectedValue = ef.IdEstadistica;
            cbEstats.SelectedValue = ef.IdEstat;
            sldQuantitat.Valor = (double)(ef.Quantitat ?? 0);
            sldDuracio.Valor = (double)(ef.Duracio ?? 0);
            if (ef.EsAfegir) rbAfegirEstat.IsChecked = true; else rbTreureEstat.IsChecked = true;
        }

        private void TipusEfecte_Changed(object sender, RoutedEventArgs e) => ActualitzarInterficieEspecífica();

        private void ActualitzarInterficieEspecífica()
        {
            if (cardEstadistica == null || cardEstat == null) return;

            if (rbModEstadistica.IsChecked == true)
            {
                cardEstadistica.Visibility = Visibility.Visible;
                cardEstat.Visibility = Visibility.Collapsed;
            }
            else
            {
                cardEstadistica.Visibility = Visibility.Collapsed;
                cardEstat.Visibility = Visibility.Visible;
            }
        }

        private void DesactivarEdicio()
        {
            _efecteEnEdicio = null;
            panelConfigEfecte.IsEnabled = false;
            panelDetallsEspecífics.IsEnabled = false;
        }

        // Mètode per "bolcar" el que hi ha als controls cap a l'objecte _efecteEnEdicio
        private void GuardarCanvisEfecteTemporal()
        {
            if (_efecteEnEdicio == null) return;

            _efecteEnEdicio.Nom = txtNomEfecte.Text;
            _efecteEnEdicio.Probabilitat = (decimal)sldProbabilitat.Valor;
            _efecteEnEdicio.Duracio = (decimal)sldDuracio.Valor;

            if (rbModEstadistica.IsChecked == true)
            {
                _efecteEnEdicio.IdEstadistica = (decimal?)cbEstadistica.SelectedValue;
                _efecteEnEdicio.Quantitat = (decimal)sldQuantitat.Valor;
                _efecteEnEdicio.IdEstat = null;
            }
            else
            {
                _efecteEnEdicio.IdEstat = (decimal?)cbEstats.SelectedValue;
                _efecteEnEdicio.EsAfegir = rbAfegirEstat.IsChecked ?? true;
                _efecteEnEdicio.IdEstadistica = null;
                _efecteEnEdicio.Quantitat = null;
            }
            lbEfectes.Items.Refresh();
        }

        #endregion

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // 1. Assegurem que l'efecte que s'està editant actualment guarda els seus valors
            GuardarCanvisEfecteTemporal();

            // 2. Validacions bàsiques
            if (string.IsNullOrWhiteSpace(txtNomAccio.Text))
            {
                MessageBox.Show("L'acció necessita un nom.");
                return;
            }

            if (_efectesLlista.Count == 0)
            {
                MessageBox.Show("Has d'afegir com a mínim un efecte (mínim 1, màxim 20).");
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    // 3. Assignem dades bàsiques de l'Acció
                    _accioActual.Nom = txtNomAccio.Text;
                    _accioActual.IdObjectiu = (decimal)cbObjectiu.SelectedValue;
                    _accioActual.Icona = "default_icon"; // IMPORTANT: Segons el teu model 'Icona' és NOT NULL

                    if (_mode == ModeFormulari.Creacio) db.Accios.Add(_accioActual);
                    else db.Update(_accioActual);

                    // GUARDEM L'ACCIÓ PRIMER per tenir ID
                    db.SaveChanges();

                    // 4. GESTIÓ DE LA TAULA ITEMS (Aquí és on va exactament)
                    // Mirem si ja existeix a la taula Items
                    var itemExistent = db.Items.FirstOrDefault(i => i.IdAccio == _accioActual.Id);

                    if (rbItem.IsChecked == true)
                    {
                        // Si l'usuari diu que és un ITEM i no existeix a la taula, el creem
                        if (itemExistent == null)
                        {
                            db.Items.Add(new Item { IdAccio = _accioActual.Id });
                        }
                    }
                    else
                    {
                        // Si l'usuari diu que és una HABILITAT, eliminem el registre d'Items si existia
                        if (itemExistent != null)
                        {
                            db.Items.Remove(itemExistent);
                        }
                    }

                    // 5. SINCRONITZACIÓ D'EFECTES
                    // Primer eliminem els efectes antics que tenia aquesta acció
                    var antics = db.Efectes.Where(ef => ef.IdAccio == _accioActual.Id);
                    db.Efectes.RemoveRange(antics);

                    // Ara afegim els de la llista actual
                    foreach (var ef in _efectesLlista)
                    {
                        // Creem una nova instància per evitar conflictes de seguiment d'Entity Framework
                        var nouEf = new Efecte
                        {
                            IdAccio = _accioActual.Id,
                            Nom = ef.Nom,
                            Probabilitat = ef.Probabilitat,
                            IdEstat = ef.IdEstat,
                            IdEstadistica = ef.IdEstadistica,
                            Quantitat = ef.Quantitat,
                            Duracio = ef.Duracio,
                            EsAfegir = ef.EsAfegir
                        };
                        db.Efectes.Add(nouEf);
                    }

                    db.SaveChanges();

                    MessageBox.Show("Acció guardada correctament!");
                    Tornar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.InnerException?.Message ?? ex.Message);
            }
        }

        private void BtnTornar_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Perdre els canvis?", "Sortir", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Tornar();
        }

        private void Tornar()
        {
            System.Windows.Navigation.NavigationService.GetNavigationService(this)?.GoBack();
        }
    }
}
