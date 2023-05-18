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
    /// Logique d'interaction pour CreerCompte.xaml
    /// </summary>
    public partial class CreerCompte : Window   // FENETRE POUR CRÉER UN NOUVEAU COMPTE
    {
        tblUtilisateur utilisateurActif;
        Guichet guichet = new Guichet();
        public CreerCompte(tblUtilisateur utilisateur)
        {
            InitializeComponent();

            this.utilisateurActif = utilisateur;

            reset();
        }

        private void reset() // Méthode pour rénitialiser notre fenetre
        {
            // Faire la liaison avec le combobox et les clients
            ComboClient.DataContext = guichet.tblUtilisateurs.ToList().Where(u => u.Admin == false);

            // Mettre nos controles dans leur état initial
            ComboClient.SelectedIndex = -1;
            ComboType.SelectedIndex = -1; 
            lblClient.Visibility = Visibility.Hidden;
            lblType.Visibility = Visibility.Hidden;

            lblMontant.Visibility = Visibility.Hidden;
            txtMontant.Text = string.Empty;
            PanelHypotheque.Visibility = Visibility.Hidden;
            PanelList.Visibility = Visibility.Hidden;

            lblErreur.Content = string.Empty;
        }

        private void Control_GotFocus(object sender, RoutedEventArgs e) // Un combobox ou textbox obtient le focus
        {
            if (sender == ComboClient)                                   // Définir le controle en question
            { 
                lblClient.Visibility = Visibility.Visible;              // L'indicateur visuel est mis à visible
            }
            else if (sender == ComboType)
            {
                lblType.Visibility = Visibility.Visible;
            }
            else if (sender == txtMontant)
            {
                lblMontant.Visibility = Visibility.Visible;
            }
        }

        private void Control_LostFocus(object sender, RoutedEventArgs e) // Un combobox ou textbox perd le focus
        {
            if (sender == ComboClient)                                   // Définir le controle en question
            {
                lblClient.Visibility = Visibility.Hidden;              // L'indicateur visuel est mis à invisible
            }
            else if (sender == ComboType)
            {
                lblType.Visibility = Visibility.Hidden;

            }
            else if (sender == txtMontant)
            {
                lblMontant.Visibility = Visibility.Hidden;
            }
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)  // Selection changed sur le combobox des types de comptes  
        {

            // Si le type choisi est Hypotheque, afficher le textbox qui demande le montant de l'hypotheque, sinon l'inverse

            if (ComboType.SelectedIndex == -1 || ComboType.SelectedValue.ToString() != "Hypothécaire")
            {
                PanelHypotheque.Visibility = Visibility.Hidden;
            }
            else
            {
                txtMontant.Text = string.Empty;
                PanelHypotheque.Visibility = Visibility.Visible;
            }
        }

        private void ComboClient_SelectionChanged(object sender, SelectionChangedEventArgs e) // Selection changed sur le combobox des clients
        {
            if (ComboClient.SelectedIndex == -1)                                            // Si aucune sélection, cacher le listbox qui affiche les comptes du client selectionné
            {
                PanelList.Visibility = Visibility.Hidden;
                return;
            }


            tblUtilisateur client = ComboClient.SelectedItem as tblUtilisateur;          // Récupérer le client

            List<tblCompte> comptes = client.tblComptes.ToList();                       // Récupérer les comptes du client

            if(comptes.Count() == 0)                                                    // Si le client n'a aucun compte, cacher le listbox
            {
                PanelList.Visibility = Visibility.Hidden;
            }
            else 
            {
                // Afficher les comptes du client 

                PanelList.Visibility = Visibility.Visible;
                ListComptes.DataContext = comptes.OrderBy(c => c.Type);
                ListComptes.SelectedIndex = -1;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) // Bouton pour revenir à la page précédente
        {
            AccueilAdministrateur fen = new AccueilAdministrateur(utilisateurActif);
            fen.Show();
            this.Close();
        }

        private void btnAnnul_Click(object sender, RoutedEventArgs e) // BOUTON ANNULER
        {
            lblErreur.Content = string.Empty;

            if (ComboClient.IsFocused)                                // Annule la selection courante
            {
                ComboClient.SelectedIndex = -1;
            }
            else if (ComboType.IsFocused) 
            { 
                ComboType.SelectedIndex = -1;
            }
            else if (txtMontant.IsFocused) 
            { 
                txtMontant.Text = string.Empty;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) // BOUTON CANCELLER
        {
            reset();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e) // BOUTON CONFIRMER
        {
            lblErreur.Content = string.Empty;

            if (ComboClient.SelectedIndex == -1 || ComboType.SelectedIndex == -1)                 // Vérifier que les combobox ont une valeur selectionné
            {
                lblErreur.Content = "Veuillez selectionner le client et le type du compte.";
                return;
            }

            string type = ComboType.SelectedValue.ToString();                                     // Le type selectionné
            tblUtilisateur client = ComboClient.SelectedItem as tblUtilisateur;                   // Le client selectionné
                                                                                                  
            if (type != "Chèque")                                                                 // Le client ne peut pas avoir d'autres comptes s'il n'a pas de compte chèque
            {
               List<tblCompte> comptesCheque = client.tblComptes.Where(c => c.Type == "C").ToList(); // Selectionner tous les comptes cheques du client

                if(comptesCheque.Count == 0)                                                         // S'il n'a pas de compte chèque
                {
                    lblErreur.Content = client.Prenom + " " + client.Nom + " ne peut pas avoir un compte " + type + ", \r\ncar il/elle ne possède pas de compte chèque.";
                    return;
                }
            }


            if (type == "Hypothécaire")                                                               // Si compte hypothequaire
            {
                if(txtMontant.Text.Trim() == string.Empty)                                            // Vérifier que le textbox qui demande le montant n'est pas vide
                {
                    lblErreur.Content = "Veuillez saisir le montant de l'hypothèque.";
                    return;
                }

                bool ok = decimal.TryParse(txtMontant.Text, out decimal montant);                   // Convertir le montant saisi en valeur numérique

                if (!ok || montant <= 0)                                                           // Si la conversion à échoué ou que le montant est plus petit ou égal à 0
                {
                    lblErreur.Content = "Le montant de l'hypothèque est invalide.";
                    return;
                }
                
            }
            else if (type == "Marge de crédit")                                                      // Un seul compte de marge de crédit par personne
            {
                List<tblCompte> compteMarge = client.tblComptes.Where(c => c.Type == "M").ToList(); // Selectionner le compte marge de crédit du client

                if (compteMarge.Count == 1)                                                         // S'il a déjà un compte marge de crédit
                {
                    lblErreur.Content = client.Prenom + " " + client.Nom + " posséde déjà un compte de marge de crédit.";
                    return;
                }
            }

            string lettre;

            switch (type)                                                            // Selectionner la premiere lettre du type
            {
                case "Épargne":
                    lettre = "E";
                    break;
                default:
                    lettre = type[0].ToString();
                    break;
            }

            tblCompte nouveau = new tblCompte();                                        // Créer l'objet Compte

            try 
            {
                nouveau.IdClient = client.ID;
                nouveau.Type = lettre;

                if(lettre == "H" )                                                    // Si compte hypothécaire 
                {
                    nouveau.Solde = decimal.Parse(txtMontant.Text.Trim());           // Solde du compte égal au montant de l'hypotheque
                }
                else
                {
                    nouveau.Solde = 0;                                                // Solde du compte à 0
                }

                guichet.tblComptes.Add(nouveau);                                      // Ajouter le compte à la table comptes
                guichet.SaveChanges();                                                // Enregistrer les changements
            }
            catch (Exception ex) 
            {
                lblErreur.Content = "Impossible de créer le compte pour le moment." ;
                Console.WriteLine(ex.Message);
                return;
            }

            //Message de confirmation
            MessageBoxResult btn = MessageBox.Show("Le compte " + type + " pour " + client.Prenom + " " + client.Nom + " a été créée. \r\nVoulez-vous créer un autre compte?", "Nouveau compte", MessageBoxButton.YesNo, MessageBoxImage.Information);
        
            if(btn == MessageBoxResult.Yes)   // Si l'utilisateur veut continuer sur cette page
            {
                reset();
                ComboClient.SelectedItem = client;
            }
            else 
            {
                AccueilAdministrateur fen = new AccueilAdministrateur(utilisateurActif);
                fen.Show();
                this.Close();
            }
        }

   
    }
}
