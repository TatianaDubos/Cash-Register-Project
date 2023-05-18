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
    /// Logique d'interaction pour AccueilClients.xaml
    /// </summary>
    public partial class AccueilClients : Window   //FENETRE D'ACCUEIL POUR LES CLIENTS
    {
        tblUtilisateur utilisateurActif;
        public AccueilClients(tblUtilisateur utilisateur)
        {
            InitializeComponent();
            this.utilisateurActif = utilisateur;

            lblNom.Content += utilisateurActif.Prenom + " " + utilisateurActif.Nom;
        }

        private void btnComptes_Click(object sender, RoutedEventArgs e) // CONSULTER LES COMPTES
        {
            ConsulterCompte fen = new ConsulterCompte(utilisateurActif, null);
            fen.Show();
            this.Close();
        }

        private void btnDepot_Click(object sender, RoutedEventArgs e) // EFFECTUER UN DEPOT
        {
            Transaction fen = new Transaction(utilisateurActif, "D");
            fen.Show();
            this.Close();
        }

        private void btnTransfert_Click(object sender, RoutedEventArgs e) // EFFECTUER UN TRANSFERT
        {
            Transfert fen = new Transfert(utilisateurActif);
            fen.Show();
            this.Close();

        }

        private void btnRetrait_Click(object sender, RoutedEventArgs e) // EFFECTUER UN RETRAIT
        {
            Transaction fen = new Transaction(utilisateurActif,"R");
            fen.Show();
            this.Close();
        }

        private void btnFacture_Click(object sender, RoutedEventArgs e) // PAYER UNE FACTURE
        {
            Transaction fen = new Transaction(utilisateurActif, "F");
            fen.Show();
            this.Close();
        }

        private void btnQuitter_Click(object sender, RoutedEventArgs e) // QUITTER
        {
            if(MessageBox.Show("Voulez-vous vraiment quitter?", "Quitter", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes) 
            {
                Login login = new Login();
                login.Show();
                this.Close();
            }
        }
    }
}
