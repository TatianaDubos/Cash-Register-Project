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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static GuichetAutomatique.Clavier;

namespace GuichetAutomatique
{
    /// <summary>
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class Login : Window  // PAGE DE CONNEXION
    {
        int compteur = 0;   // Compte le nombre d'essais de connexion échoué
        public Login()
        {
            InitializeComponent();
            reset();
        }

        private void reset() // Méthode pour rénitialiser notre fenêtre
        {
            // Mettre nos controles dans leur état initial
            txtID.Text = "ID";
            txtID.Foreground = Brushes.LightGray;
            txtNIP.Password = string.Empty;
            lblIDtxt.Visibility = Visibility.Hidden;
            lblNIPtxt.Visibility = Visibility.Hidden;
            lblID.Visibility = Visibility.Hidden;
            lblNIP.Visibility = Visibility.Hidden;
            lblErreur.Content = string.Empty;
        }

        private void textbox_GotFocus(object sender, RoutedEventArgs e)   // TEXBOX obtient le focus
        {
            if (sender == txtID)                                          //TEXTBOX ID
            {
                lblID.Visibility = Visibility.Visible;                   // Indicateur visuel est mis à visible
                lblIDtxt.Visibility = Visibility.Visible;                // Label du textbox affiché

                if (txtID.Text == "ID")                                 // Si le placeholder est présent
                {
                    txtID.Text = string.Empty;                          // Effacer le placeholder
                    txtID.Foreground = Brushes.Black;
                }
            }
            else if (sender == txtNIP)                                  // TEXTBOX NIP
            {
                lblNIP.Visibility = Visibility.Visible;                 //Indicateur visuel est mis a visible
                lblNIPtxt.Visibility = Visibility.Visible;              //Label du textbox est affiché
            }
        }
        private void textbox_LostFocus(object sender, RoutedEventArgs e)   // TEXBOX perd le focus
        {
            if (sender == txtID)                                           // TEXTBOX ID
            {
                lblID.Visibility = Visibility.Hidden;                     // Indicateur visuel est mis à invisible
                lblIDtxt.Visibility = Visibility.Hidden;                  // Label du textbox est vidé

                if (string.IsNullOrWhiteSpace(txtID.Text))                // Si le textbox est vide
                {
                    txtID.Text = "ID";                                    // Remettre le placeholder
                    txtID.Foreground = Brushes.LightGray;
                }
            }
            else if (sender == txtNIP)                                   //TEXTBOX NIP
            {
                lblNIP.Visibility = Visibility.Hidden;                   //Indicateur visuel est mis a invisible
                lblNIPtxt.Visibility = Visibility.Hidden;                //Label du texbox est vidé
            }
        }

        private void Clavier_Click(object sender, RoutedEventArgs e)  // CLICK SUR UN BOUTON DU CLAVIER VISUEL
        {
            Button btn = sender as Button;

            if (txtID.IsFocused) // Si txtID a le focus
            {
                Clavier.aff(txtID, btn.Content.ToString()); 
            }
            else if (txtNIP.IsFocused) // Si txtNIP a le focus
            {
                Clavier.aff(txtNIP, btn.Content.ToString());
            }
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)  // BOUTON CONFIRMER
        {
            lblErreur.Content = string.Empty;

            if (txtID.Text.Trim() == string.Empty || txtNIP.Password.Trim() == string.Empty) // Vérifier que les champs ont une valeur
            {
                lblErreur.Content = "Veuillez renseigner tous les champs";
                return;
            }

            Guichet guichet = new Guichet();

            int id;
            bool ok = int.TryParse(txtID.Text, out id);

            if (!ok)  // Si la conversion de ID en int a échoué
            {
                lblErreur.Content = "ID invalide";
                return;
            }

            tblUtilisateur utilisateur = guichet.tblUtilisateurs.Find(id);          // Rechercher le ID saisi dans la table utilisateur

            if(utilisateur == null)                                                 // Vérifier que le ID existe
            {
                lblErreur.Content = "ID invalide";
                return;
            }

            if(utilisateur.NIP != txtNIP.Password.Trim())                          // Verifier que le NIP est valide
            {
                lblErreur.Content = "NIP invalide";
                compteur++;                                                      

                if (compteur >= 3)                                                // Si le nombre d'essais a été dépassé
                {
                    lblErreur.Content = "Vos trois essais ont échoué, l'accès a votre compte à été bloqué";

                    try 
                    {
                        utilisateur.Acces = false;                                 // Bloquer l'utilisateur
                        guichet.SaveChanges();                                     // Enregistrer les changements
                    }
                    catch(Exception ex) 
                    {
                        lblErreur.Content = "Une erreur est survenue";
                        Console.WriteLine(ex.Message);
                    }
                }

                return;
            }

            if (!utilisateur.Acces)                                              // Verifier que l'acces de l'utilisateur est autorise
            {
                lblErreur.Content = "L'accès a votre compte à été bloqué, veuillez contacter votre banque";
                return;
            }

            if (utilisateur.Admin)                                                 // Déterminer si l'utilisateur est un admin ou un client                
            {
                AccueilAdministrateur fen = new AccueilAdministrateur(utilisateur); // Afficher la fenetre d'accueil admin
                fen.Show();
            }
            else 
            {
                AccueilClients fen = new AccueilClients(utilisateur);               // Afficher la fenetre d'accueil client
                fen.Show();
            }
            this.Close();
        }

        private void btnAnnul_Click(object sender, RoutedEventArgs e) // BOUTON ANULLER
        {
            lblErreur.Content = string.Empty;

            if (txtID.IsFocused)                                      // Effacer la valeur saisie dans le champ qui a le focus
            {
                txtID.Text = string.Empty;                                       
             
            }
            else if (txtNIP.IsFocused)
            {
                txtNIP.Password = string.Empty;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) // BOUTON CANCELLER
        {
            reset();                                                    // Effacer toutes les valeurs saisies
          
        }

        private void txtID_TextChanged(object sender, TextChangedEventArgs e) // Evenement TextChanged sur le textbox ID
        {
            compteur = 0;                                                     // Met le compteur d'essais à 0
        }

        private void Passwordbox_KeyDown(object sender, KeyEventArgs e)       // Evenement KeyDown sur le passwordbox NIP 
        {
            if (e.Key == Key.Enter)                                           // Touche enter
            {
                e.Handled = true;                                              // Empêche la touche "Enter" de produire son effet habituel
                btnConfirm.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // Clique sur le bouton confirmer
            }
        }
    }
}
