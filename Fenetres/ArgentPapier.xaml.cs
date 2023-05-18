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
using static GuichetAutomatique.Clavier;

namespace GuichetAutomatique
{
    /// <summary>
    /// Logique d'interaction pour ArgentPapier.xaml
    /// </summary>
    public partial class ArgentPapier : Window         // FENETRE POUR CONSULTER L'ARGENT PAPIER RESTANT DANS LE GUICHET
    {
        tblUtilisateur utilisateurActif;
        Guichet guichet = new Guichet();
        tblGuichet argent = new tblGuichet();
        public ArgentPapier(tblUtilisateur utilisateur)
        {
            InitializeComponent();
            this.utilisateurActif = utilisateur;

            reset();
        }
        private void reset() // Méthode pour rénitialiser notre fenetre
        {
            // Mettre nos controles dans leur état initial
            txtMontant.Text = string.Empty;
            lblMontant.Visibility = Visibility.Hidden;
            lblErreur.Content = string.Empty;

            // Afficher l'argent papier qu'il reste dans le guichet
            argent = guichet.tblGuichets.First();

            lblArgentPapier.Content = "CONTIENT :  " + String.Format("{0:c}", argent.ArgentPapier) ;

        }
         private void txtMontant_GotFocus(object sender, RoutedEventArgs e)  // Le textbox obtient le focus
        {
            lblMontant.Visibility = Visibility.Visible;                     // L'indicateur visuel est mis à visible
        }

        private void txtMontant_LostFocus(object sender, RoutedEventArgs e)  // Le textbox perd le focus
        {
            lblMontant.Visibility = Visibility.Hidden;                       // L'indicateur visuel est mis à invisible
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) // Bouton pour revenir à la page précédente
        {
            AccueilAdministrateur fen = new AccueilAdministrateur(utilisateurActif);
            fen.Show();
            this.Close();
        }

        private void Clavier_Click(object sender, RoutedEventArgs e)  // CLICK SUR UN BOUTON DU CLAVIER VISUEL
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
           
            argent = guichet.tblGuichets.First();

            if (argent.ArgentPapier >= 20000)                            // Vérifier si le guichet peut recevoir de l'argent papier (limite de 20 000$)
            {
                lblErreur.Content = "La réserve du guichet est remplie au maximum.";
                return; 
            }

            int montant;
            bool ok = int.TryParse(txtMontant.Text, out montant);      // Conversion du montant en int

            if (!ok || montant <= 0)                                  // Si la conversion à échoué ou si le montant est 0
            {
                lblErreur.Content = "Montant invalide.";
                return;
            }

            if ((argent.ArgentPapier + montant) > 20000)                // Vérifier que l'ajout du montant à l'argent du guichet ne dépassera pas la limite permise
            {
                lblErreur.Content = "Il ne peut pas y avoir plus que 20 000$ dans la réserve du guichet.";
                return;
            }

         
            try 
            {
                argent.ArgentPapier += montant;                          // Ajouter le montant

                guichet.SaveChanges();                                  // Enregistrer les changements
            }
            catch (Exception ex) 
            {
                lblErreur.Content = "Impossible d'effectuer cette action pour le moment.";
                Console.WriteLine(ex.Message);
                return;
            }

            lblArgentPapier.Content = "CONTIENT :  " + String.Format("{0:c}", argent.ArgentPapier) ;      // Afficher le montant obtenu et un message de confirmation

            MessageBoxResult btn = MessageBox.Show("Vous avez ajouté " + String.Format("{0:c}", montant) + ".\r\nLe guichet contient maintenant " + String.Format("{0:c}", argent.ArgentPapier) + " en réserve. \r\nDésirez-vous ajouter plus d'argent?", "Guichet", MessageBoxButton.YesNo, MessageBoxImage.Information);
        
            if(btn == MessageBoxResult.Yes)                                                 // Si l'utilisateur veut continuer 
            {
                reset();
            }
            else 
            {
                AccueilAdministrateur fen = new AccueilAdministrateur(utilisateurActif);
                fen.Show();
                this.Close();
            }
        }

        private void btnAnnul_Click(object sender, RoutedEventArgs e) // BOUTON ANNULER
        {
            lblErreur.Content = string.Empty;

            if (txtMontant.IsFocused)                                // Efface le contenu du textbox
            { 
                txtMontant.Text = string.Empty;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) // BOUTON CANCELLER
        {
            reset();
        }

        private void txtMontant_KeyDown(object sender, KeyEventArgs e)            // Evenement KeyDown sur le textbox du montant
        {
                if (e.Key == Key.Enter)                                            // Touche enter 
                {
                    e.Handled = true;                                              // Empêche la touche "Enter" de produire son effet habituel 
                    btnConfirm.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // Clique sur le bouton confirmer
                }
        }
    }
}
