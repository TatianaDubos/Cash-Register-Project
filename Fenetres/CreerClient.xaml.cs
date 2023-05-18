using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace GuichetAutomatique
{
    /// <summary>
    /// Logique d'interaction pour CreerClient.xaml
    /// </summary>
    public partial class CreerClient : Window              // FENETRE POUR CRÉER UN CLIENT
    {
        tblUtilisateur utilisateurActif;
        PasswordBox pass = new PasswordBox();
       
        public CreerClient(tblUtilisateur utilisateur)
        {
            InitializeComponent();

            this.utilisateurActif = utilisateur;

            // Creation d'un passwordbox pour le nip
            pass.Height = 40;
            pass.Width = 300;
            pass.PasswordChar = '*';
            pass.FontSize = 24;
            pass.MaxLength = 4;
            pass.HorizontalContentAlignment = HorizontalAlignment.Center;
            pass.VerticalContentAlignment = VerticalAlignment.Center;
            pass.GotFocus += PassBox_GotFocus;
            pass.LostFocus += PassBox_LostFocus;
            pass.KeyDown += PassBox_KeyDown;
            pass.Name = "passNIP";
        }

        private void PassBox_KeyDown(object sender, KeyEventArgs e)           // Evenement KeyDown sur le passwordbox
        {
            if (e.Key == Key.Enter)                                           // Touche enter 
            {
                e.Handled = true;                                              // Empêche la touche "Enter" de produire son effet habituel 
                btnConfirm.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // Clique sur le bouton confirmer
            }
        }

        private void reset()    // Méthode pour rénitialiser notre fenetre
        {
            // Mettre les controles dans leur état initial
            txtPrenom.Text = "Prénom";
            txtPrenom.Foreground = Brushes.Gray;
            txtNom.Text = "Nom";
            txtNom.Foreground = Brushes.Gray;
            txtTelephone.Text = "Téléphone";
            txtTelephone.Foreground = Brushes.Gray;
            txtCourriel.Text = "Courriel";
            txtCourriel.Foreground = Brushes.Gray;

            if (!PanelNip.Children.Contains(txtNIP) && PanelNip.Children.Contains(pass))
            {
                pass.Password = string.Empty;
                PanelNip.Children.Remove(pass);
                PanelNip.Children.Add(txtNIP);
            }

            txtNIP.Text = "NIP";
            txtNIP.Foreground = Brushes.Gray;


            lblPrenom.Visibility = Visibility.Hidden;
            lblNom.Visibility = Visibility.Hidden;
            lblTelephone.Visibility = Visibility.Hidden;
            lblCourriel.Visibility = Visibility.Hidden;
            lblNIP.Visibility = Visibility.Hidden;

            lblErreur.Content = string.Empty;

        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e) // UN TEXBOX reçoit le focus
        {
            TextBox textBox = sender as TextBox;
            switch (textBox.Name)                                      // Trouver quel est ce textbox
            {                                                          // Appeller la méthode gotfocus avec les bons parametres
                case "txtPrenom":
                    gotfocus(txtPrenom, lblPrenom, "Prénom");
                    break;
                case "txtNom":
                    gotfocus(txtNom, lblNom, "Nom");
                    break;
                case "txtTelephone":
                    gotfocus(txtTelephone, lblTelephone, "Téléphone");
                    break;
                case "txtCourriel":
                    gotfocus(txtCourriel, lblCourriel, "Courriel");
                    break;
                case "txtNIP":
                    PanelNip.Children.Remove(txtNIP);                  //Remplacer le textbox par un passwordbox
                    if (!PanelNip.Children.Contains(pass))
                    { 
                        PanelNip.Children.Add(pass);
                        pass.Focus();                                   //Le passwordbox obtient le focus
                    }
                    break;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e) // UN TEXTBOX perd le focus
        {
               TextBox textbox = sender as TextBox;

            switch (textbox.Name)                                       // Trouver quel est ce textbox
            {                                                           // Appeller la méthode lostfocus avec les bons parametres
                case "txtPrenom":
                    lostfocus(txtPrenom, lblPrenom, "Prénom");
                    break;
                case "txtNom":
                    lostfocus(txtNom, lblNom, "Nom");
                    break;
                case "txtTelephone":
                    lostfocus(txtTelephone, lblTelephone, "Téléphone");
                    break;
                case "txtCourriel":
                     lostfocus(txtCourriel, lblCourriel, "Courriel");
                    break;
            }
        }

        private void gotfocus(TextBox textBox, System.Windows.Controls.Label label, string text)   // Méthode qui s'occupe d'un texbox qui obtient le focus
        {
            label.Visibility = Visibility.Visible;                                                 // Indicateur visuel est visible

            if (textBox.Text == text)                                                              // Si un placeholder est présent
            {
                textBox.Text = string.Empty;                                                       // Effacer le placeholder
                textBox.Foreground = Brushes.Black;
            }
        }

        private void lostfocus(TextBox textBox, System.Windows.Controls.Label label, string text) // Méthode qui s'occupe d'un textbox qui perd le focus
        {
            label.Visibility = Visibility.Hidden;                                                 // Indicateur visuel est invisible

            if (string.IsNullOrWhiteSpace(textBox.Text))                                          // Si le texbox est vide
            {
                textBox.Text = text;                                                              // Remettre le placeholder
                textBox.Foreground = Brushes.Gray;
            }

        }
        private void PassBox_GotFocus(object sender, RoutedEventArgs e) // PASSWORDBOX a obtenu le focus
        {
            lblNIP.Visibility = Visibility.Visible;
        }
        private void PassBox_LostFocus(object sender, RoutedEventArgs e) // PASSWORDBOX a perdu le focus
        {
            if (pass.Password.Trim() == "")
            {
                PanelNip.Children.Remove(pass);         // Si aucun caractere a été saisi,
                PanelNip.Children.Add(txtNIP);          // on remplace le passwordbox par un textbox
                lostfocus(txtNIP, lblNIP, "NIP");       // le texbox a perdu le focus
            }
          
            lblNIP.Visibility = Visibility.Hidden;
        }
        private void btnBack_Click(object sender, RoutedEventArgs e) // Bouton pour revenir à la page précédente
        {
            AccueilAdministrateur fen = new AccueilAdministrateur(utilisateurActif);
            fen.Show();
            this.Close();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e) // BOUTON CONRFIRMER
        {   
            lblErreur.Content = string.Empty;

            string prenom, nom, telephone, courriel, nip;

            prenom = txtPrenom.Text.Trim();                             // Récupérer les données saisies
            nom = txtNom.Text.Trim();
            telephone = txtTelephone.Text.Trim();
            courriel = txtCourriel.Text.Trim();
            nip = pass.Password.Trim();
                                                                        // Verifier si nos variables contiennent bien une valeur
            if(prenom == string.Empty || nom == string.Empty || telephone == string.Empty || courriel == string.Empty || nip == string.Empty)
            {
                lblErreur.Content = "Veuillez renseigner tous les champs.";
                return;
            }

            string error = string.Empty;

            if (!int.TryParse(nip, out int num))                      // Vérifier qu'il y a uniquement des nombres dans le nip 
            {
                error = "Le NIP doit être une valeur numérique.";
            }
            else if (nip.Length != 4)                                // Vérifier qu'il y a 4 caracteres dans le nip 
            {
                error = "Le NIP doit contenir quatre nombres.";
            }

            if (!Regex.IsMatch(courriel, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) // Vérifier que le courriel contient un @
            {
                if (error == string.Empty) error = "Le courriel est invalide.";
                else error += "\r\nLe courriel est invalide.";
            }

            if (!Regex.IsMatch(telephone, @"^\d{3}-\d{3}-\d{4}$"))  // Vérifier que le numéro de téléphone est dans le bon format
            {
                if (error == string.Empty) error = "Le numéro de téléphone doit être dans le format : 000-000-0000.";
                else error += "\r\nLe numéro de téléphone doit être dans le format : 000-000-0000.";
            }

            if (error != string.Empty)                                // Afficher les erreurs s'il y en a
            {
                lblErreur.Content = error;
                return;
            }

            // S'il n'y a aucune erreur dans les informations saisies
            Guichet guichet = new Guichet();
            tblUtilisateur nouveau = new tblUtilisateur();                 // Créer un objet de type Utilisateur

            try
            {                                                             // Met la premiere lettre en majuscule et le reste en minuscule
                prenom = prenom[0].ToString().ToUpper() + prenom.Substring(1).ToLower();
                nom = nom[0].ToString().ToUpper() + nom.Substring(1).ToLower();

                nouveau.Prenom = prenom;                                  // Initialiser les propriétés du nouveau Utilisateur
                nouveau.Nom = nom;
                nouveau.Telephone = telephone;
                nouveau.Courriel = courriel;
                nouveau.NIP = nip;
                nouveau.Acces = true;

                guichet.tblUtilisateurs.Add(nouveau);                     // Ajouter le nouvel utilisateur dans le DataSet du guichet
                guichet.SaveChanges();                                    // Enregistrer les changements
            }
            catch (Exception ex)
            { 
                lblErreur.Content = "Impossible d'enregistrer le nouveau client pour le moment.";
                Console.WriteLine(ex.Message);
                return;
            }

            MessageBoxResult btn = MessageBox.Show(prenom + " " + nom + " a été ajouté, son identifiant est " + nouveau.ID + ". \r\nVoulez-vous ajouter un autre client?", "Nouveau  client", MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (btn == MessageBoxResult.Yes)                           // Si l'admin veut ajouter un autre client
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
            if (txtPrenom.IsFocused)                                   // Effacer le contenu du champ qui a le focus
            { 
                txtPrenom.Text = string.Empty;
            }
            else if (txtNom.IsFocused)
            {
                txtNom.Text = string.Empty;
            }
            else if (txtCourriel.IsFocused)
            {
                txtCourriel.Text = string.Empty;
            }
            else if (txtTelephone.IsFocused)
            {
                txtTelephone.Text = string.Empty;
            }
            else if (pass.IsFocused)
            {
                pass.Password = string.Empty;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) // BOUTON CANCEL
        {
            reset();                                                   // Met la fenetre dans son état initial
        }

    }
}

