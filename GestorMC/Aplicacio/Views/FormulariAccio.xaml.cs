using Microsoft.EntityFrameworkCore;
using Model.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Aplicacio.Views
{
    public partial class FormulariAccio : UserControl
    {
        private ModeFormulari _mode;
        private int? _idAccio;
        private Accio _accioActual;

        private ObservableCollection<Efecte> _efectesLlista = new ObservableCollection<Efecte>();
        private Efecte _efecteEnEdicio;

        // Control per evitar bucles infinits a l'hora de carregar i actualitzar UI
        private bool _isLoadingEffect = false;

        public FormulariAccio(ModeFormulari mode, int? idAccio = null)
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
                    cbObjectiu.ItemsSource = db.Objectius.ToList();
                    cbObjectiu.DisplayMemberPath = "Nom";
                    cbObjectiu.SelectedValuePath = "Id";

                    cbEstadistica.ItemsSource = db.TaulaEstadistiques.ToList();
                    cbEstadistica.DisplayMemberPath = "Nom";
                    cbEstadistica.SelectedValuePath = "Id";

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
                    _accioActual = db.Accios
                        .Include(a => a.Efectes)
                        .Include(a => a.Habilitat)
                        .Include(a => a.Item)
                        .FirstOrDefault(a => a.Id == _idAccio);

                    if (_accioActual != null)
                    {
                        txtIdAccio.Text = _accioActual.Id.ToString();
                        txtNomAccio.Text = _accioActual.Nom;

                        txtDescEfecte.Text = _accioActual.Descripcio;
                        cbObjectiu.SelectedValue = _accioActual.IdObjectiu;

                        rbHabilitat.IsChecked = _accioActual.Habilitat != null;
                        rbItem.IsChecked = _accioActual.Item != null;

                        foreach (var e in _accioActual.Efectes) _efectesLlista.Add(e);

                        if (_efectesLlista.Count > 0)
                        {
                            lbEfectes.SelectedItem = _efectesLlista[0];
                            PrepararEdicioEfecte(_efectesLlista[0]);
                        }
                    }
                }
                else
                {
                    _accioActual = new Accio();
                    txtIdAccio.Text = "NOVA";
                    panelConfigEfecte.IsEnabled = false; // Desactivat fins que es creï un efecte
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

            var nouEfecte = new Efecte { Nom = "Nou Efecte", Probabilitat = 100, Quantitat = 0, Duracio = 0, EsAfegir = true };
            _efectesLlista.Add(nouEfecte);

            lbEfectes.SelectedItem = nouEfecte;
            PrepararEdicioEfecte(nouEfecte);
        }

        // AFEGIT: S'executa sempre que canvies la selecció al ListBox
        private void LbEfectes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Si no estem enmig d'una càrrega i tenim un efecte editant-se, el guardem en memòria
            if (!_isLoadingEffect && _efecteEnEdicio != null)
            {
                GuardarCanvisEfecteTemporal();
            }

            // Un cop l'antic està guardat a la llista, preparem el nou que s'ha seleccionat
            if (lbEfectes.SelectedItem is Efecte seleccionat)
            {
                PrepararEdicioEfecte(seleccionat);
            }
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

        // AFEGIT: Sincronitza el camp de text amb la llista a temps real
        private void TxtNomEfecte_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Ignorem si s'està escrivint per codi durant la càrrega inicial d'un efecte
            if (_isLoadingEffect || _efecteEnEdicio == null) return;

            _efecteEnEdicio.Nom = txtNomEfecte.Text;
            lbEfectes.Items.Refresh(); // Força el refresc visual del ListBox
        }

        private void PrepararEdicioEfecte(Efecte ef)
        {
            _isLoadingEffect = true; // Bloquegem events temporalment
            _efecteEnEdicio = ef;

            panelConfigEfecte.IsEnabled = true;
            panelDetallsEspecífics.IsEnabled = true;

            txtNomEfecte.Text = ef.Nom;
            sldProbabilitat.Valor = (double)(ef.Probabilitat ?? 100);

            if (ef.IdEstat != null) rbCanviEstat.IsChecked = true;
            else rbModEstadistica.IsChecked = true;

            ActualitzarInterficieEspecífica();

            cbEstadistica.SelectedValue = ef.IdEstadistica;
            cbEstats.SelectedValue = ef.IdEstat;
            sldQuantitat.Valor = (double)(ef.Quantitat ?? 0);
            sldDuracio.Valor = (double)(ef.Duracio ?? 0);
            if (ef.EsAfegir) rbAfegirEstat.IsChecked = true; else rbTreureEstat.IsChecked = true;

            _isLoadingEffect = false; // Desbloquegem
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
            _isLoadingEffect = true;
            _efecteEnEdicio = null;
            panelConfigEfecte.IsEnabled = false;
            panelDetallsEspecífics.IsEnabled = false;
            txtNomEfecte.Text = "";
            _isLoadingEffect = false;
        }

        private void GuardarCanvisEfecteTemporal()
        {
            if (_efecteEnEdicio == null) return;

            _efecteEnEdicio.Nom = txtNomEfecte.Text;
            _efecteEnEdicio.Probabilitat = (int)sldProbabilitat.Valor;
            _efecteEnEdicio.Duracio = (int)sldDuracio.Valor;

            if (rbModEstadistica.IsChecked == true)
            {
                _efecteEnEdicio.IdEstadistica = (int?)cbEstadistica.SelectedValue;
                _efecteEnEdicio.Quantitat = (int)sldQuantitat.Valor;
                _efecteEnEdicio.IdEstat = null;
            }
            else
            {
                _efecteEnEdicio.IdEstat = (int?)cbEstats.SelectedValue;
                _efecteEnEdicio.EsAfegir = rbAfegirEstat.IsChecked ?? true;
                _efecteEnEdicio.IdEstadistica = null;
                _efecteEnEdicio.Quantitat = null;
            }
            lbEfectes.Items.Refresh();
        }

        #endregion

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Ens assegurem que el que estem tocant ara mateix també queda guardat a memòria abans de fer l'INSERT a DB
            GuardarCanvisEfecteTemporal();

            if (string.IsNullOrWhiteSpace(txtNomAccio.Text))
            {
                MessageBox.Show("L'acció necessita un nom.", "Atenció", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbObjectiu.SelectedValue == null)
            {
                MessageBox.Show("L'acció necessita un Objectiu (Aliat, Enemic...).", "Falta l'objectiu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_efectesLlista.Count == 0)
            {
                MessageBox.Show("Has d'afegir com a mínim un efecte (mínim 1, màxim 20).", "Atenció", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    if (_mode == ModeFormulari.Edicio && _idAccio.HasValue)
                    {
                        _accioActual = db.Accios.Include(a => a.Efectes).FirstOrDefault(a => a.Id == _idAccio);
                    }

                    _accioActual.Nom = txtNomAccio.Text;
                    _accioActual.Descripcio = txtDescEfecte.Text;
                    _accioActual.IdObjectiu = (int)cbObjectiu.SelectedValue;
                    _accioActual.Icona = "default_icon"; // Pots implementar pujada d'icona en el futur

                    if (_mode == ModeFormulari.Creacio) db.Accios.Add(_accioActual);
                    else db.Update(_accioActual);

                    // Guardem l'acció per garantir que tenim una ID generada si és nova
                    db.SaveChanges();

                    // Gestió d'Item/Habilitat
                    var itemExistent = db.Items.FirstOrDefault(i => i.IdAccio == _accioActual.Id);
                    if (rbItem.IsChecked == true)
                    {
                        if (itemExistent == null) db.Items.Add(new Item { IdAccio = _accioActual.Id });
                    }
                    else
                    {
                        if (itemExistent != null) db.Items.Remove(itemExistent);
                    }

                    // MODIFICAT: Esborrem de cop els antics i inserim els nous per evitar violacions de PRIMARY KEY
                    var antics = db.Efectes.Where(ef => ef.IdAccio == _accioActual.Id);
                    db.Efectes.RemoveRange(antics);
                    db.SaveChanges(); // Esborrem físicament primer

                    foreach (var ef in _efectesLlista)
                    {
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

                    // Actualitzem l'estat local a "Edició" per si hem partit de mode Creació, de forma que no tanqui la pantalla
                    _mode = ModeFormulari.Edicio;
                    _idAccio = _accioActual.Id;
                    txtIdAccio.Text = _accioActual.Id.ToString();

                    MessageBox.Show("Acció i efectes guardats correctament!", "Èxit", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + (ex.InnerException?.Message ?? ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnTornar_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Vols tornar? Qualsevol canvi no guardat es perdrà.", "Sortir", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Tornar();
            }
        }

        private void Tornar()
        {
            System.Windows.Navigation.NavigationService.GetNavigationService(this)?.GoBack();
        }
    }
}