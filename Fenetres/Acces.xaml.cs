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
    /// Logique d'interaction pour Acces.xaml
    /// </summary>
    public partial class Acces : Window      // FENTRE POUR MODIFIER L'ACCES D'UN CLIENT
    {
        tblUtilisateur utilisateurActif;
        Guichet guichet = new Guichet();
      
        public Acces(tblUtilisateur utilisateur)
        {
            InitializeComponent();

            this.utilisateurActif = utilisateur;

            reset();
        }

        private void reset() // Méthode pour rénitialiser notre fenetre
        {
            // Établir la liaison entre les données à afficher et le combobox
            if (btnTous.IsEnabled == false)  // Bouton Afficher tous les clients
            {
                ComboClient.DataContext = guichet.tblUtilisateurs.ToList().Where(u => u.Admin == false); 
            }
            else if (btnAutorise.IsEnabled == false) // Bouton Afficher tous les clients autorisés
            {
                ComboClient.DataContext = guichet.tblUtilisateurs.ToList().Where(u => u.Admin == false && u.Acces == true);
            }
            else if (btnNonAutorise.IsEnabled == false) // Bouton Afficher tous les clients non autorisés
            {
                ComboClient.DataContext = guichet.tblUtilisateurs.ToList().Where(u => u.Admin == false && u.Acces == false);
            }

            // Mettre nos controles dans leur état initial
            ComboClient.SelectedIndex = -1;
            lblClient.Visibility = Visibility.Hidden;
            lblAcces.Visibility = Visibility.Hidden;
            RadioAutorise.IsChecked = false;
            RadioNonAutorise.IsChecked = false;
            RadioAutorise.FontWeight = FontWeights.Normal;
            RadioNonAutorise.FontWeight = FontWeights.Normal;
            RadioAutorise.Foreground = Brushes.Black;
            RadioNonAutorise.Foreground = Brushes.Black;
            RadioAutorise.FontSize = 16;
            RadioNonAutorise.FontSize = 16;
            lblErreur.Content = string.Empty;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) // Bouton pour revenir à la page précédente
        {
            AccueilAdministrateur fen = new AccueilAdministrateur(utilisateurActif);
            fen.Show();
            this.Close();
        }

        private void Control_GotFocus(object sender, RoutedEventArgs e)   // Un controle obtient le focus
        {
            if(sender == ComboClient) 
            {
                lblClient.Visibility = Visibility.Visible;                // Mettre l'indicateur visuel à visible
            }
            else if(sender == RadioAutorise || sender == RadioNonAutorise) 
            {
                lblAcces.Visibility = Visibility.Visible;
            }
        }

        private void Control_LostFocus(object sender, RoutedEventArgs e) // Un controle perd le focus
        {
            if (sender == ComboClient)
            {
                lblClient.Visibility = Visibility.Hidden;                // Mettre l'indicateur visuel à invisible
            }
            else if (sender == RadioAutorise || sender == RadioNonAutorise)
            {
                lblAcces.Visibility = Visibility.Hidden;
            }
        }

        private void btnAnnul_Click(object sender, RoutedEventArgs e)   // BOUTON ANNULER
        {
            lblErreur.Content = string.Empty;

            if (ComboClient.SelectedIndex == -1) return;

            tblUtilisateur selection = ComboClient.SelectedItem as tblUtilisateur;   // Récupérer l'utilisateur selectionné

            switch (selection.Acces)                                                 // Selectionner le bon radiobutton (annule les actions qui n'ont pas été confirmé) 
            {
                case true:
                    RadioAutorise.IsChecked = true;
                    break;
                case false:
                    RadioNonAutorise.IsChecked = true;
                    break;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) // BOUTON CANCELLER
        {
            reset();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e) // BOUTON CONFIRMER
        {
            lblErreur.Content = string.Empty;

            if (ComboClient.SelectedIndex == -1) 
            {
                lblErreur.Content = "Veuillez sélectionner un client.";
                return; }

            tblUtilisateur selection = ComboClient.SelectedItem as tblUtilisateur;      // Récupérer l'utilisateur selectionné

            bool acces;                              // Initialiser la variable acces avec la valeur du radiobutton selectionné
            string texte;

            if (RadioAutorise.IsChecked == true) 
            {
                acces = true;
                texte = "autorisé";
            }
            else 
            { 
                acces = false;
                texte = "interdit";
            }

            if(selection.Acces == acces)                 // Si l'acces du client a déjà la valeur voulu
            {
                lblErreur.Content = "L'accès au client " + selection.Prenom + " " + selection.Nom + " est déjà " + texte + ".";
                return;
            }

            // Demander à l'utilisateur si il désire réellement modifier l'acces du client
            MessageBoxResult btn = MessageBox.Show("Voulez-vous réellement modifier l'accès de " + selection.Prenom + " " + selection.Nom + "?", "Modifier l'accès", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (btn != MessageBoxResult.Yes)   // Si l'utilisateur refuse
            {
                reset();
                ComboClient.SelectedItem = selection;

                return;
            }

            try
            {
                selection.Acces = acces;            // Modifier l'acces du client selectionné
                guichet.SaveChanges();              // Enregister les modifications
            }
            catch (Exception ex)
            {
                lblErreur.Content = "Impossible d'effactuer cette action pour le moment.";
                Console.WriteLine(ex.ToString());
                return;
            }

            // Afficher un message de confirmation
            MessageBox.Show("L'accès à " + selection.Prenom + " " + selection.Nom + " est dorénavant " + texte + ".", "Modifier l'accès", MessageBoxButton.OK, MessageBoxImage.Information);

            if(btnTous.IsEnabled) reset();     // Mettre a jour la liste des clients si le bouton Tous les clients est activé 
        }

        private void ComboClient_SelectionChanged(object sender, SelectionChangedEventArgs e)  // COMBOBOX événement Selection Changed 
        {
            lblErreur.Content = string.Empty;

            if (ComboClient.SelectedIndex == -1) {
                PanelRadios.IsEnabled = false;                                                  // Si acune selection, désactiver les radiobuttons 
                return; 
            }

            PanelRadios.IsEnabled = true;                                                      // Activer les radiobuttons 

            tblUtilisateur selection = ComboClient.SelectedItem as tblUtilisateur;             // Récupérer l'utilisateur selectionné
             
            switch (selection.Acces)                                                           // Selectionner le radiobutton correspondant
            {
                case true:
                    RadioAutorise.IsChecked = true;
                    break;
                case false:
                    RadioNonAutorise.IsChecked = true;
                    break;
            }

        }

        private void RadioAutorise_Checked(object sender, RoutedEventArgs e)   // radiobutton autorisé est selectionné
        {
            // Mettre en évidence le radiobutton selectionné
            RadioAutorise.FontWeight = FontWeights.Bold;
            RadioAutorise.Foreground = Brushes.Green;
            RadioAutorise.FontSize = 18;
            RadioNonAutorise.FontWeight= FontWeights.Normal;
            RadioNonAutorise.Foreground = Brushes.Black;
            RadioNonAutorise.FontSize = 16;
        }

        private void RadioNonAutorise_Checked(object sender, RoutedEventArgs e) // radiobutton non autorisé est selectionné
        {
            // Mettre en évidence le radiobutton selectionné
            RadioNonAutorise.FontWeight = FontWeights.Bold;
            RadioNonAutorise.Foreground = Brushes.Red;
            RadioNonAutorise.FontSize = 18;
            RadioAutorise.FontWeight = FontWeights.Normal;
            RadioAutorise.Foreground = Brushes.Black;
            RadioAutorise.FontSize = 16;
        }

        private void btn_Click(object sender, RoutedEventArgs e) // Click sur un des trois bouton qui déterminent les clients à afficher dans le combobox
        {
            // Desactiver le bouton qui a été cliqué et activer les autres boutons

            if (sender == btnTous)  // Bouton Afficher tous les clients
            {
                btnTous.IsEnabled = false;                                                               
                btnAutorise.IsEnabled = true;
                btnNonAutorise.IsEnabled = true;
            }
            else if (sender == btnAutorise) // Bouton Afficher tous les clients autorisés
            {
                btnTous.IsEnabled = true;
                btnAutorise.IsEnabled = false;
                btnNonAutorise.IsEnabled = true;
            }
            else if (sender == btnNonAutorise) // Bouton Afficher tous les clients non autorisés
            {
                btnTous.IsEnabled = true;
                btnAutorise.IsEnabled = true;
                btnNonAutorise.IsEnabled = false;
            }

            reset(); // Appeler reset() pour faire notre liaison entre nos données et le combobox
        }
    }
}