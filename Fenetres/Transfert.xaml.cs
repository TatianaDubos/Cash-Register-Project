using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
using static GuichetAutomatique.Clavier;

namespace GuichetAutomatique
{
    /// <summary>
    /// Logique d'interaction pour Transfert.xaml
    /// </summary>
    public partial class Transfert : Window   // FENETRE POUR LES TRANSFERTS
    {
        Guichet guichet = new Guichet();
        tblUtilisateur utilisateurActif;
        public Transfert(tblUtilisateur utilisateur)
        {
            InitializeComponent();
            this.utilisateurActif = utilisateur;

            reset();
        }

        private void reset() // Méthode pour rénitialiser la fenêtre
        {
            // Liaison de nos données avec nos combobox
            ComboCompteDepart.DataContext = utilisateurActif.tblComptes.Where(c => c.Type == "C" ||  c.Type == "E").OrderBy(c => c.Type).ToList(); // Comptes cheque ou épargne seulement
            ComboCompteDestination.DataContext = utilisateurActif.tblComptes.OrderBy(c => c.Type).ToList();                                         // Tous les comptes
         
            // Mettre nos controles dans leur état initial
            lblErreur.Content = string.Empty;
            ComboCompteDepart.SelectedIndex = -1;
            ComboCompteDestination.SelectedIndex = -1;
            txtMontant.Text = string.Empty;
            lblCompteDepart.Visibility = Visibility.Hidden;
            lblCompteDestination.Visibility = Visibility.Hidden;
            lblMontant.Visibility = Visibility.Hidden;

            btnSoldeDepart.IsEnabled = false;
            btnSoldeDepart.Content = "Solde";
            btnSoldeDestination.IsEnabled = false;
            btnSoldeDestination.Content = "Solde";

        }
        private new void GotFocus(object sender, RoutedEventArgs e) // Un TextBox ou ComboBox obtient le focus
        {
            if (sender == ComboCompteDepart)                    // Définir quel est le controle en question
            {
                lblCompteDepart.Visibility = Visibility.Visible;    // Indicateur visuel mis à visible
            }
            if (sender == ComboCompteDestination) 
            {
                lblCompteDestination.Visibility = Visibility.Visible;
            }
            if (sender == txtMontant) 
            {
                lblMontant.Visibility = Visibility.Visible;
            }

        }
        private new void LostFocus(object sender, RoutedEventArgs e) // Un TextBox ou ComboBox perd le focus
        {
            if (sender == ComboCompteDepart)                        // Définir quel est le controle en question
            {
                lblCompteDepart.Visibility = Visibility.Hidden;    // Indicateur visuel mis à invisible
            }
            if (sender == ComboCompteDestination)
            {
                lblCompteDestination.Visibility = Visibility.Hidden;   
            }
            if (sender == txtMontant)
            {
                lblMontant.Visibility = Visibility.Hidden;          
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) // Bouton pour revenir à la page précédente
        {
            AccueilClients fen = new AccueilClients(utilisateurActif);
            fen.Show();
            this.Close();
        }

        private void ComboCompteDepart_SelectionChanged(object sender, SelectionChangedEventArgs e)  // Selection changed sur le 1e combobox
        {
            btnSoldeDepart.Content = "Solde";           // Rénitialiser le 1e bouton Solde

            if (ComboCompteDepart.SelectedIndex == -1)  // Aucune selection dans le 1e combobox
            {
                btnSoldeDepart.IsEnabled = false;       // Bouton solde désactivé

                ComboCompteDestination.DataContext = utilisateurActif.tblComptes.OrderBy(c => c.Type).ToList();  // Afficher tous les comptes dans le 2e combobox

                return; 
            }
                                                    // Un élément est selectionné dans le 1e combobox
            btnSoldeDepart.IsEnabled = true;        // Bouton solde activé

            // VERIFICATIONS POUR ÉVITER QUE COMPTE DEPART ET COMPTE DESTINATION DÉSIGNE LE MEME COMPTE

            if(ComboCompteDestination.SelectedIndex == -1)                              // Aucun élément est selectionné dans le 2e combobox
            {
                // Retirer du 2e combobox l'élement selectionné dans le 1e combobox
                ComboCompteDestination.DataContext = utilisateurActif.tblComptes.Where(c => c != ComboCompteDepart.SelectedItem).OrderBy(c => c.Type).ToList();
            }
            else                                                                        // Un élément est selectionné dans le 2e combobox
            {
                if(ComboCompteDestination.SelectedItem == ComboCompteDepart.SelectedItem) // L'élément selectionné dans le 2e combobox est égal à l'élément selectionné dans le 1e combobox
                {
                    // Retirer du 2e combobox l'élement selectionné dans le 1e combobox
                    // Aucun élément ne sera selectionné
                    ComboCompteDestination.DataContext = utilisateurActif.tblComptes.Where(c => c != ComboCompteDepart.SelectedItem).OrderBy(c => c.Type).ToList();
                    ComboCompteDestination.SelectedIndex = -1;
                }
                else                                                                       // L'élément selectionné dans le 2e combobox est différent à l'élément selectionné dans le 1e combobox
                {   // Récupérer le compte selectionné
                    tblCompte compte =  ComboCompteDestination.SelectedItem as tblCompte;  

                    // Retirer du 2e combobox l'élement selectionné dans le 1e combobox
                    // Re-selectionner le compte voulu
                    ComboCompteDestination.DataContext = utilisateurActif.tblComptes.Where(c => c != ComboCompteDepart.SelectedItem).OrderBy(c => c.Type).ToList();
                    ComboCompteDestination.SelectedItem = compte;

                }
            }
        }

        private void ComboCompteDestination_SelectionChanged(object sender, SelectionChangedEventArgs e) // Selection changed sur le 2e combobox
        {
            btnSoldeDestination.Content = "Solde";           // Rénitialiser 2e bouton solde

            if (ComboCompteDestination.SelectedIndex == -1) // Aucune selection dans le 2e combobox
            {
                btnSoldeDestination.IsEnabled = false;      // Bouton solde désactivé
                return;
            }
                                                            // un élément est selectionné dans le 2e combobox
            btnSoldeDestination.IsEnabled = true;          // Bouton solde activé

        }

        private void btnSolde_Click(object sender, RoutedEventArgs e)   // Click sur le bouton pour afficher le solde
        {

            Button btnSolde = sender as Button;                // Récupérer le bouton cliqué
            
                if (btnSolde.Content.ToString() == "Solde")   // Si le bouton Solde est dans son état initial
                {
                    if(btnSolde == btnSoldeDepart)             // Bouton associé au 1e combobox
                    {
                        tblCompte compte = ComboCompteDepart.SelectedItem as tblCompte;    // Récupérer le compte sélectionné
                        btnSolde.Content = String.Format("{0:c}", compte.Solde) ;           // Afficher le solde du compte
                    }
                    else if (btnSolde == btnSoldeDestination) // Bouton associé au 2e combobox
                    {
                        tblCompte compte = ComboCompteDestination.SelectedItem as tblCompte;    // Récupérer le compte sélectionné
                         btnSolde.Content = String.Format("{0:c}", compte.Solde) ;               // Afficher le solde du compte
                    } 
                }
                else
                {
                    btnSolde.Content = "Solde";               // Cacher le solde du compte
                }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) // BOUTON CANCELLER
        {
            reset();
        }

        private void btnAnnul_Click(object sender, RoutedEventArgs e) // BOUTON ANNULER
        {
            lblErreur.Content = string.Empty;

            if (ComboCompteDepart.IsFocused)                          // Vider le controle selectionné
            {
                ComboCompteDepart.SelectedIndex = -1;
            }
            else if(ComboCompteDestination.IsFocused) 
            {
                ComboCompteDestination.SelectedIndex = -1;
            }
            else if (txtMontant.IsFocused) 
            {
                txtMontant.Text = string.Empty;
            }
        }

        private void Clavier_Click(object sender, RoutedEventArgs e) // Clic sur un bouton du clavier numérique
        {

            if (txtMontant.IsFocused) // Si le textbox a le focus
            {
                Button btn = sender as Button;
                Clavier.aff(txtMontant, btn.Content.ToString());
            }
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e) // BOUTON CONFIRMER
        {
            lblErreur.Content = string.Empty;

            if(ComboCompteDepart.SelectedIndex == -1                   // Vérifier que toutes les informations ont été saisies
                || ComboCompteDestination.SelectedIndex == -1
                || txtMontant.Text.Trim() == string.Empty)  
            {
                lblErreur.Content = "Veuillez saisir les informations demandées.";
                return;
            }

            bool ok = decimal.TryParse(txtMontant.Text, out decimal montant); // Conversion du montant en decimal

            if (!ok || montant <= 0)                                        // La conversion du montant en decimal à échoué ou le montant est plus petit ou égal à 0
            {
                lblErreur.Content = "Montant invalide.";
                return;
            }

            tblCompte compteDepart = ComboCompteDepart.SelectedItem as tblCompte;  // Récupérer le compte de départ

            if(compteDepart.Solde < montant)                                      // Vérifier que le solde est suffisant pour faire le transfert
            {
                lblErreur.Content = compteDepart.Aff + " ne contient pas assez de fond pour effectuer un transfert de " + String.Format("{0:c}", montant) ; 
                return;
            }

            tblCompte compteDestination = ComboCompteDestination.SelectedItem as tblCompte;  // Récupérer le compte de destination

            if(compteDestination.Type == "M")   // Si compteDestination est une marge de credit
            {
                if(compteDestination.Solde <= 0)  // Vérifier que le solde est supérieur à 0
                {
                    lblErreur.Content = compteDestination.Aff + " ne contient aucune dette à rembourser.";
                    return;
                }

                if (compteDestination.Solde < montant) // Vérifier que le solde est plus grand ou égal au montant
                {
                    lblErreur.Content = compteDestination.Aff + " contient une dette de " + String.Format("{0:c}", compteDestination.Solde) + ".\r\nLe montant du remboursement ne peut pas être supérieur au montant de la dette.";
                    return;
                }
            }

            try 
            {
                // Effectuer le transfert
                compteDepart.Solde -= montant;

                string tranType = string.Empty;

                switch (compteDestination.Type)
                {
                    case "M":
                        compteDestination.Solde -= montant;             // Diminuer le solde si compte marge de crédit
                        tranType = "C";                                  // Transaction de type remboursement
                        break;
                    case "C":                                            
                    case "E":
                    case "H":
                        compteDestination.Solde += montant;             // Augmenter le solde si compte éparge,cheque ou hypotheque
                        tranType = "T";                                 // Transaction de type transfert
                        break;
                }

                // Ajouter la transaction dans l'historique des transactions
                guichet.addTransaction(compteDepart.IdCompte, compteDestination.IdCompte, montant, tranType);

                //S'assurer que les modifications ont été effectué dans la table comptes
                guichet.tblComptes.AddOrUpdate(compteDepart);
                guichet.tblComptes.AddOrUpdate(compteDestination);

                //Enregistrer les changements
                guichet.SaveChanges();
            }
            catch(Exception ex) 
            { 
                Console.WriteLine(ex.Message);
                lblErreur.Content = "Impossible d'effectuer un transfert pour le moment.";
                return;
            }

            // Message de confirmation
            MessageBoxResult btn = MessageBox.Show("Vous avez transféré " + String.Format("{0:c}", montant) + " dans le compte " + compteDestination.Aff + ". \r\nDésirez-vous effectuer un autre transfert?", "Transferts", MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (btn == MessageBoxResult.Yes)                                      // Si l'utilisateur veut continuer 
            {
                reset();
            }
            else
            {
                AccueilClients fen = new AccueilClients(utilisateurActif);
                fen.Show();
                this.Close();
            }

        }

        private void txtMontant_KeyDown(object sender, KeyEventArgs e)             // Evenement KeyDown sur le textbox du montant
        {
                if (e.Key == Key.Enter)                                            // Touche enter 
                {
                    e.Handled = true;                                              // Empêche la touche "Enter" de produire son effet habituel 
                    btnConfirm.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // Clique sur le bouton confirmer
                }
        }
    }
}
