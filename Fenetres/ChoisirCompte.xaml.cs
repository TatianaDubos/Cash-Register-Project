using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GuichetAutomatique
{
    /// <summary>
    /// Logique d'interaction pour ChoisirCompte.xaml
    /// </summary>
    public partial class ChoisirCompte : Window   // FENETRE POUR CHOISIR UN COMPTE À CONSULTER (ADMINISTRATEUR SEULEMENT)
    {
        tblUtilisateur utilisateurActif;
        Guichet guichet = new Guichet();
        public ChoisirCompte(tblUtilisateur utilisateur)
        {
            InitializeComponent();
            this.utilisateurActif = utilisateur;

            reset();
        }

        public void reset()    // Méthode pour rénitialiser la fenetre
        {
            // Établir les liaisons entre les combobox et nos données
            ComboCheque.DataContext = guichet.tblComptes.Where(c => c.Type == "C").OrderBy(c => c.tblUtilisateur.Nom).ToList();
            ComboEpargne.DataContext = guichet.tblComptes.Where(c => c.Type == "E").OrderBy(c => c.tblUtilisateur.Nom).ToList();
            ComboHypothecaire.DataContext = guichet.tblComptes.Where(c => c.Type == "H").OrderBy(c => c.tblUtilisateur.Nom).ToList();
            ComboMarge.DataContext = guichet.tblComptes.Where(c => c.Type == "M").OrderBy(c => c.tblUtilisateur.Nom).ToList();

            //Mettre nos controles dans leur état initial
            ComboCheque.SelectedIndex = -1;
            lblCheque.Visibility = Visibility.Hidden;
            ComboEpargne.SelectedIndex = -1;
            lblEpargne.Visibility = Visibility.Hidden;
            ComboHypothecaire.SelectedIndex = -1;
            lblHypothecaire.Visibility = Visibility.Hidden;
            ComboMarge.SelectedIndex = -1;
            lblMarge.Visibility = Visibility.Hidden;
        }

        private void ComboBox_GotFocus(object sender, RoutedEventArgs e)  // Un ComboBox a obtenu le focus
        {
            ComboBox comboBox = sender as ComboBox;                       // Obtenir le combobox en question
            switch (comboBox.Name)                                        // Rendre son indicateur visuel visible
            {
                case "ComboCheque": 
                    lblCheque.Visibility = Visibility.Visible;
                    break;
                case "ComboEpargne":
                    lblEpargne.Visibility = Visibility.Visible;
                    break;
                case "ComboHypothecaire":
                    lblHypothecaire.Visibility = Visibility.Visible;
                    break;
                case "ComboMarge":
                    lblMarge.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void ComboBox_LostFocus(object sender, RoutedEventArgs e) // Un ComboBox a perdu le focus
        {
            ComboBox comboBox = sender as ComboBox;                       // Obtenir le combobox en question
            switch (comboBox.Name)                                        // Rendre son indicateur visuel invisible
            {
                case "ComboCheque":
                    lblCheque.Visibility = Visibility.Hidden;
                    break;
                case "ComboEpargne":
                    lblEpargne.Visibility = Visibility.Hidden;
                    break;
                case "ComboHypothecaire":
                    lblHypothecaire.Visibility = Visibility.Hidden;
                    break;
                case "ComboMarge":
                    lblMarge.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) // Bouton pour revenir à la page précédente
        {
            AccueilAdministrateur fen = new AccueilAdministrateur(utilisateurActif);
            fen.Show();
            this.Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)   // La selection d'un combobox a changé
        {
            ComboBox combo = sender as ComboBox;

            if(combo.SelectedIndex == -1) return;

            ConsulterCompte fen = new ConsulterCompte(utilisateurActif, combo.SelectedItem as tblCompte);  // Afficher la fenetre Consulter compte
            fen.Show();
            this.Close();
        }
    }
}
