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
    /// Logique d'interaction pour ConsulterCompte.xaml
    /// </summary>
    public partial class ConsulterCompte : Window     // FENTRE POUR CONSULTER UN COMPTE CHOISI
    {
        tblUtilisateur utilisateurActif;
        Guichet guichet = new Guichet();
        tblCompte choix;
    
        public ConsulterCompte(tblUtilisateur utilisateur, tblCompte compte)
        {
            InitializeComponent();
            this.utilisateurActif = utilisateur;

           

            if (compte != null)                     // Si l'utilisateur(administrateur) a choisi un compte dans ChoisirCompte.xaml
            {
                choix = compte ;                   
            }
            
            reset();

        }

        private void reset()                       // Méthode pour rénitialiser notre fenetre
        {
            if (choix != null)                      // Si l'utilisateur(admin) a choisi un compte dans ChoisirCompte.xaml
            {
                ComboCompte.DisplayMemberPath = "AffAdmin";
                ComboCompte.DataContext = guichet.tblComptes.Where(c => c.Type == choix.Type).OrderBy(c => c.tblUtilisateur.Nom).ToList();    // Afficher tous les comptes du type choisi
                ComboCompte.SelectedValue = choix.IdCompte; // Selectionner le compte que l'admin a choisi
            }
            else
            {                                          // Si l'utilisateur est un client : afficher tous ses comptes
                ComboCompte.DataContext = utilisateurActif.tblComptes.OrderBy(c => c.Type).ToList();
                ComboCompte.SelectedIndex = -1;
            }

            // Mettre les controles dans leur état initial
            lblSolde.Content = "SOLDE : ";
            ListTransactions.SelectedIndex = -1;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) // Bouton pour revenir à la page précédente
        {
            if (utilisateurActif.Admin)
            { 
                ChoisirCompte fen = new ChoisirCompte(utilisateurActif);
                fen.Show();
            }
            else 
            { 
                AccueilClients fen = new AccueilClients(utilisateurActif);
                fen.Show();
            }

            this.Close();
        }

        private void ComboCompte_SelectionChanged(object sender, SelectionChangedEventArgs e)  // COMBOBOX EVENEMENT SELECTION CHANGED
        {
            if (ComboCompte.SelectedIndex == -1) return;


            tblCompte selection = (tblCompte)ComboCompte.SelectedItem ;                        // Récupérer le compte selectionné

            lblSolde.Content = "SOLDE :  " + String.Format("{0:c}", selection.Solde);          // Afficher le solde

            List<tblTransaction> transactions = new List<tblTransaction>();
            transactions = selection.tblTransactions.Concat(selection.tblTransactions1).ToList(); // Récupérer toutes nos transactions (concaténation des deux collections CompteDepart et CompteDestination pour les transfert qui sont lié a deux comptes différent )

            ListTransactions.DataContext = transactions.OrderByDescending(t => t.Date);          // Lier notre Listbox avec nos transactions
            ListTransactions.SelectedIndex = -1;
        }

    }
}
