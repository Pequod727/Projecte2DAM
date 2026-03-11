using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Model.Models;

namespace Aplicacio.Views
{
    public enum ModeFormulari { Creacio, Edicio, Vista }

    public partial class FormulariPersonatge : UserControl
    {
        private ModeFormulari _mode;
        private decimal? _idPersonatge;
        private Personatge _personatgeActual;

        // Constructor sense paràmetres (útil pel dissenyador de WPF)
        public FormulariPersonatge() : this(ModeFormulari.Creacio, null) { }

        // Constructor principal on li passem el mode i la ID opcional
        public FormulariPersonatge(ModeFormulari mode, decimal? idPersonatge = null)
        {
            InitializeComponent();
            _mode = mode;
            _idPersonatge = idPersonatge;

            ConfigurarInterficie();
            CarregarDades();
        }

        private void ConfigurarInterficie()
        {
            bool esEditable = (_mode == ModeFormulari.Creacio || _mode == ModeFormulari.Edicio);

            // Bloquegem o desbloquem els inputs (excepte l'ID que sempre està bloquejat)
            txtNom.IsReadOnly = !esEditable;
            txtDescripcio.IsReadOnly = !esEditable;
            txtVida.IsReadOnly = !esEditable;
            txtAtac.IsReadOnly = !esEditable;
            txtDefensa.IsReadOnly = !esEditable;
            txtVelocitat.IsReadOnly = !esEditable;
            txtExperiencia.IsReadOnly = !esEditable;
            chkJugable.IsEnabled = esEditable;

            // Mostrem o amaguem botons segons el mode
            if (_mode == ModeFormulari.Vista)
            {
                txtTitol.Text = "FITXA DEL PERSONATGE";
                btnGuardar.Visibility = Visibility.Collapsed;
                btnEditar.Visibility = Visibility.Visible;
            }
            else if (_mode == ModeFormulari.Edicio)
            {
                txtTitol.Text = "EDITAR PERSONATGE";
                btnGuardar.Visibility = Visibility.Visible;
                btnEditar.Visibility = Visibility.Collapsed;
            }
            else // Creacio
            {
                txtTitol.Text = "NOU PERSONATGE";
                btnGuardar.Visibility = Visibility.Visible;
                btnEditar.Visibility = Visibility.Collapsed;
            }
        }

        private void CarregarDades()
        {
            if (_mode != ModeFormulari.Creacio && _idPersonatge.HasValue)
            {
                using (var db = new AppDbContext())
                {
                    _personatgeActual = db.Personatges.Find(_idPersonatge.Value);

                    if (_personatgeActual != null)
                    {
                        txtId.Text = _personatgeActual.Id.ToString();
                        txtNom.Text = _personatgeActual.Nom;
                        txtDescripcio.Text = _personatgeActual.Descripcio;
                        txtVida.Text = _personatgeActual.Vida.ToString();
                        txtAtac.Text = _personatgeActual.Atac.ToString();
                        txtDefensa.Text = _personatgeActual.Defensa.ToString();
                        txtVelocitat.Text = _personatgeActual.Velocitat.ToString();
                        txtExperiencia.Text = _personatgeActual.Experiencia.ToString();
                        chkJugable.IsChecked = _personatgeActual.Jugable;
                    }
                }
            }
            else
            {
                // Si és creació, preparem un objecte buit
                _personatgeActual = new Personatge();
            }
        }

        // --- BOTONS D'ACCIÓ ---
        private void BtnCanviarAEdicio_Click(object sender, RoutedEventArgs e)
        {
            // Passem de Vista a Edició directament
            _mode = ModeFormulari.Edicio;
            ConfigurarInterficie();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validació simple
            if (string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtVida.Text))
            {
                MessageBox.Show("Siusplau, omple els camps obligatoris.", "Dades incompletes", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    // Convertim els valors (Si l'usuari posa text on van números, fallarà, s'hauria de validar millor per a producció)
                    _personatgeActual.Nom = txtNom.Text.Trim();
                    _personatgeActual.Descripcio = txtDescripcio.Text.Trim();
                    _personatgeActual.Vida = decimal.Parse(txtVida.Text);
                    _personatgeActual.Atac = decimal.Parse(txtAtac.Text);
                    _personatgeActual.Defensa = decimal.Parse(txtDefensa.Text);
                    _personatgeActual.Velocitat = decimal.Parse(txtVelocitat.Text);
                    _personatgeActual.Experiencia = decimal.Parse(txtExperiencia.Text);
                    _personatgeActual.Jugable = chkJugable.IsChecked ?? false;

                    // Imatge obligatòria a la base de dades. Com no tenim selector, hi posem un array buit temporalment
                    if (_personatgeActual.Imatge == null)
                        _personatgeActual.Imatge = new byte[0];

                    if (_mode == ModeFormulari.Creacio)
                    {
                        db.Personatges.Add(_personatgeActual);
                    }
                    else
                    {
                        db.Personatges.Update(_personatgeActual);
                    }

                    db.SaveChanges();
                    MessageBox.Show("Dades guardades correctament!", "Èxit", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Un cop guardat, tornem a la llista
                    NavegarEnrere();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar les dades: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnTornar_Click(object sender, RoutedEventArgs e)
        {
            if (_mode == ModeFormulari.Creacio || _mode == ModeFormulari.Edicio)
            {
                MessageBoxResult res = MessageBox.Show(
                    "Estàs segur que vols sortir? Es perdran els canvis no guardats.",
                    "Confirmació",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (res != MessageBoxResult.Yes) return;
            }

            NavegarEnrere();
        }

        private void NavegarEnrere()
        {
            // Com que és un UserControl, utilitzem GetNavigationService per buscar el Frame que ens engloba
            var ns = NavigationService.GetNavigationService(this);
            ns?.Navigate(new VistaPersonatges());
        }
    }
}