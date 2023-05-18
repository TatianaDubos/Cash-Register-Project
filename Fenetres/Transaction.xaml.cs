using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static GuichetAutomatique.Clavier;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;

namespace GuichetAutomatique
{
    /// <summary>
    /// Logique d'interaction pour Transaction.xaml
    /// </summary>
    public partial class Transaction : Window    // FENETRE POUR LES TRANSACTIONS : Depot, Retrait, Factures
    {
        tblUtilisateur utilisateurActif;
        Guichet guichet = new Guichet();
        Image distributeur =  new Image();
        static string chemin = "C:\\Users\\purex\\source\\repos\\GuichetAutomatiquePI1\\GuichetAutomatique\\Fenetres\\";
        string fonction;           // Nous informeras de qu'est ce que l'utilisateur veut faire sur cette fenetre
                                   // (Depot, Retrait, Payer une facture)
        public Transaction(tblUtilisateur utilisateur, string fonction)
        {
            InitializeComponent();
            this.utilisateurActif = utilisateur;
            this.fonction = fonction;

            reset();

            switch (fonction)              // Changer le titre de notre page selon sa fonction
            {
                case "D":
                    this.Title = "Dépôt";
                    lblTitre.Content = "Dépôt";
                    break;
                case "R":
                    this.Title = "Retrait";
                    lblTitre.Content = "Retrait";

                    // Ajouter le distributeur de billet
                    distributeur.Source = new BitmapImage(new Uri(chemin + "distributeur.png"));
                    AnimationPanel.Children.Add(distributeur);
                    distributeur.Margin = new Thickness(0, -100, 0, 0);

                    break;
                case "F":
                    this.Title = "Payer une facture";
                    lblTitre.Content = "Payer une facture";
                    break;
            }
        }

        private void reset()  // Méthode pour rénitialiser la fenetre
        {
            switch (fonction)  // Faire la laison entre le combobox et les données selon la fonction de la fenetre
            {
                case "D":
                    ComboCompte.DataContext = utilisateurActif.tblComptes.Where(c => c.Type != "M").OrderBy(c => c.Type).ToList();           // Dépot dans n'importe quel compte sauf marge de crédit
                    break;
                case "R":
                    ComboCompte.DataContext = utilisateurActif.tblComptes.Where(c => c.Type == "C" || c.Type == "E").OrderBy(c => c.Type); // Retrait dans un compte cheque ou epargne
                    break;
                case "F":
                    ComboCompte.DataContext = utilisateurActif.tblComptes.Where(c => c.Type == "C").OrderBy(c => c.Type);                  // Payer une facture avec un compte cheque uniquement
                    break;
            }

            // Mettre nos controles dans leur état initial
            lblErreur.Content = string.Empty;
            ComboCompte.SelectedIndex = -1;
            txtMontant.Text = string.Empty;
            lblCompte.Visibility = Visibility.Hidden;
            lblMontant.Visibility = Visibility.Hidden;
            btnSolde.IsEnabled = false;
            btnSolde.Content = "Solde";
            Panel.IsEnabled = true;                             
            btnBack.IsEnabled = true;
        }

        static List<List<int>> FindCombinations(int amount)  // Algorithme qui trouve des combinaisons de billets possible pour un montant
        {
            int[] bills = new int[] { 10, 20, 50, 100 };
            int numBills = bills.Length;

            int[,] dp = new int[amount + 1, numBills];

            for (int j = 0; j < numBills; j++)
            {
                dp[0, j] = 1;
            }

            for (int i = 1; i <= amount; i++)
            {
                for (int j = 0; j < numBills; j++)
                {
                    int include = i >= bills[j] ? dp[i - bills[j], j] : 0;
                    int exclude = j > 0 ? dp[i, j - 1] : 0;
                    dp[i, j] = include + exclude;
                }
            }

            List<List<int>> combinations = new List<List<int>>();
            FindCombinationsHelper(dp, amount, bills, numBills - 1, new List<int>(), combinations);

            List<List<int>> three = new List<List<int>>();    // On choisi une, deux ou trois possibilités selon le nombre de choix

            int num = combinations.Count();

            if(num == 1)        // Une seule combinaison possible
            {
                three.Add(combinations[0]);
            }
            else if(num == 2)   // Deux combinaisons possible
            {
                three.Add(combinations[0]);
                three.Add(combinations[1]);
            }
            else if( num >= 3)   // Plus de trois combinaisons possible
            {
                three.Add(combinations[0]); 

                if(num % 2 == 0) 
                    three.Add(combinations[num / 2]);
                else 
                    three.Add(combinations[(int)((num / 2)-0.5)]);

                three.Add(combinations[num - 1]);
            }
           
            return three;
        }
        // Methode qui aide l'algorithme précédant à trouver les combinaisons de billets pour un montant
        static void FindCombinationsHelper(int[,] dp, int amount, int[] bills, int j, List<int> combination, List<List<int>> combinations) 
        {
            if (amount == 0)
            {
                combinations.Add(combination);
                return;
            }

            if (j < 0 || amount < 0)
            {
                return;
            }

            for (int count = 0; count <= dp[amount, j]; count++)
            {
                List<int> newCombination = new List<int>(combination);
                for (int k = 0; k < count; k++)
                {
                    newCombination.Add(bills[j]);
                }
                FindCombinationsHelper(dp, amount - count * bills[j], bills, j - 1, newCombination, combinations);
            }
        }

      
        public async void animation(List<int> billets)  // ANIMATION POUR DISTRIBUER LES BILLETS LORS D'UN RETRAIT
        {
            if (billets == null)
            {
                return;
            }

            DoubleAnimation animation = new DoubleAnimation   // Transition qui augmente l'opacité d'une image
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.5)
            };

            Image newImage;
            BitmapImage myBitmap = null;

            for (int i = 0; i < billets.Count(); i++)
            {
                switch (billets[i])                    // Determiner le billet à afficher pour chaque int dans la collections de billets
                {
                    case 10:
                        myBitmap = new BitmapImage(new Uri(chemin + "10dollars.png"));
                        break;
                    case 20:
                        myBitmap = new BitmapImage(new Uri(chemin + "20dollars.png"));
                        break;
                    case 50:
                        myBitmap = new BitmapImage(new Uri(chemin + "50dollars.png"));
                        break;
                    case 100:
                        myBitmap = new BitmapImage(new Uri(chemin + "100dollars.png"));
                        break;
                }

                newImage = new Image                       // Propriété de l'image
                {
                    Source = myBitmap,
                    Width = 120,
                    Margin = new Thickness(5, -35, 5, 0),
                    Cursor = Cursors.Hand
                };
                StackPanel.SetZIndex(newImage, i);
                newImage.MouseLeftButtonDown += new MouseButtonEventHandler(Image_Click);  

                AnimationPanel.Children.Insert(1, newImage);  // Ajouter l'image dans le stackpanel
                newImage.BeginAnimation(UIElement.OpacityProperty, animation);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
        private async void Image_Click(object sender, MouseButtonEventArgs e) // EVENEMENT CLICK SUR UNE IMAGE CRÉEE PAR L'ANIMATION LORS D'UN RETRAIT
        {
            Image image = (Image)sender;

            DoubleAnimation animation = new DoubleAnimation();         // Animation de transition qui diminue l'opacité de l'image
            animation.From = 1.0;
            animation.To = 0.0;
            animation.Duration = TimeSpan.FromSeconds(0.5);
            image.BeginAnimation(UIElement.OpacityProperty, animation);
            await Task.Delay(TimeSpan.FromSeconds(0.5));

            AnimationPanel.Children.Remove(image);                     // Retirer l'image

            if (AnimationPanel.Children.Count == 1)                    // Si toutes les images de billet ont été retiré du stackpanel
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5));


                // Message de confirmation
                MessageBoxResult btn = MessageBox.Show("Désirez-vous effectuer un autre retrait?", "Retrait", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (btn == MessageBoxResult.Yes)                        // Si l'utilisateur veut continuer 
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
        }

        private void Got_Focus(object sender, RoutedEventArgs e)  // Le ComboBox ou le Textbox a obtenu le focus
        {
            if (sender == ComboCompte)                            // On definit le controle qui a le focus
            {
                lblCompte.Visibility = Visibility.Visible;        // L'indicateur visuel est mis à visible
            }
            else if (sender == txtMontant)
            {
                lblMontant.Visibility = Visibility.Visible;
            }
        }
        private void Lost_Focus(object sender, RoutedEventArgs e)  // Le ComboBox ou le Textbox a perdu le focus
        {

            if (sender == ComboCompte)                            // On definit le controle qui a perdu le focus
            {
                lblCompte.Visibility = Visibility.Hidden;        // L'indicateur visuel est mis à invisible
            }
            else if (sender == txtMontant)
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


        private void ComboCompte_SelectionChanged(object sender, SelectionChangedEventArgs e) // Un item a été selectionné dans le combobox
        {
            btnSolde.Content = "Solde";          // Rénitialiser le texte du bouton solde

            if (ComboCompte.SelectedIndex == -1)
            {
                btnSolde.IsEnabled = false;   // Bouton Solde désactivé si aucune selection 
                return;
            }

            btnSolde.IsEnabled = true;       // Bouton Solde activé 
        }

        private void btnSolde_Click(object sender, RoutedEventArgs e)       // Click sur le bouton pour afficher le solde
        {
            if (btnSolde.Content.ToString() == "Solde")                    // Si le Bouton est dans son état initial
            {
                tblCompte compte = ComboCompte.SelectedItem as tblCompte;  // Récupérer le compte sélectionné
                btnSolde.Content = String.Format("{0:c}", compte.Solde) ;  // Afficher le solde du compte
            }
            else 
            {
                btnSolde.Content = "Solde";                                 // Cacher le solde du compte
            }
        }

        private void btnAnnul_Click(object sender, RoutedEventArgs e) // BOUTON ANNULER
        {
            lblErreur.Content = string.Empty;

            if (ComboCompte.IsFocused)                              // Le controle qui a le focus est vidé
            { 
                ComboCompte.SelectedIndex = -1;
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

        private void Clavier_Click(object sender, RoutedEventArgs e)  // Clic sur un bouton du clavier numérique
        {

            if (txtMontant.IsFocused) // Si le textbox a le focus
            {
                Button btn = sender as Button;
                Clavier.aff(txtMontant, btn.Content.ToString());
            }
        }
   
        private void nouvDepot(decimal montant, tblCompte compte)      // Méthode pour effectuer un dépot
        {
            try 
            {   
                compte.Solde += montant;                              // Augmenter le solde du compte

                guichet.tblComptes.AddOrUpdate(compte);               //S'assurer que les modifications ont été effectué dans la table comptes

                guichet.addTransaction(compte.IdCompte, 0, montant, "D");// Ajouter la transaction dans l'historique des transactions

                guichet.SaveChanges();                                 //Enregistrer les changements
            }
            catch(Exception ex) 
            {
                lblErreur.Content = "Impossible d'effectuer un dépot pour le moment.";
                Console.WriteLine(ex.Message);
                return;
            }

            // Message de confirmation
            MessageBoxResult btn = MessageBox.Show("Vous avez déposé " + String.Format("{0:c}", montant) + " dans le compte " + compte.Aff + ". \r\nDésirez-vous effectuer un autre dépôt?", "Dépôt", MessageBoxButton.YesNo, MessageBoxImage.Information);

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

        private void nouvRetrait(decimal montant, tblCompte compte)    // Méthode pour effectuer un retrait
        {
            if (!(montant % 10 == 0))                                 // Vérifier que le montant est un multiple de 10
            {
                lblErreur.Content = "Les retraits sont limités aux montants finissant par 0.";
                return;
            }

            if(montant > 1000)                                       // Vérifier que le montant ne dépasse pas 1000$
            {
                lblErreur.Content = "Impossible de retirer plus de 1 000$ par transaction de retrait.";
                return;
            }

            tblGuichet argentpapier = guichet.tblGuichets.First();      // Argent papier du guichet

            if(argentpapier.ArgentPapier < montant)                    // Vérifier s'il reste assez d'argent papier dans le guichet
            {
                lblErreur.Content = "Ce guichet ne contient pas assez d'argent papier pour effectuer un retrait de " + String.Format("{0:c}", montant) + " pour le moment.";
                return;
            }

            if (compte.Solde < montant)                               // Vérifier que le solde est suffisant                   
            {                                                        
                if (utilisateurActif.tblComptes.Where(c => c.Type == "M").Count() == 0) //Si le client ne posséde pas un compte de marge de crédit
                {                                                                      // Refuser la transaction
                    lblErreur.Content = compte.Aff + " ne contient pas assez de fond pour effectuer un retrait de " + String.Format("{0:c}", montant);
                    return;
                }
                                                                                      //Le client posséde une marge de crédit
                                                                                    // Demander au client s'il souhaite puiser dans son compte marge de crédit
                MessageBoxResult btn = MessageBox.Show(compte.Aff + " ne contient pas assez de fond pour effectuer un retrait de " + String.Format("{0:c}", montant) + "\r\nVoulez-vous puiser dans la marge de crédit?", "Retrait", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (btn != MessageBoxResult.Yes)                                    // Le client refuse      
                {
                    lblErreur.Content = "Transaction annulé.";            
                    return;
                }

                // Le client souhaite puiser dans sa marge de crédit
                decimal soldeTemp = compte.Solde;
                decimal difference = montant - soldeTemp;               // Calcule la différence                                                        

                try
                {                                                       // Récupérer le compte marge
                    tblCompte credit = utilisateurActif.tblComptes.Where(c => c.Type == "M").First() as tblCompte;

                    compte.Solde = 0;                                  // Solde du compte à 0
                    credit.Solde += difference;                        // Ajouter la différence au solde de la marge de crédit
                    argentpapier.ArgentPapier -= montant;              // Soustraire le montant du retrait à l'argent papier du guichet

                                                                      // Ajouter les transactions dans l'historique des transactions
                    if (soldeTemp != 0)
                    {
                        guichet.addTransaction(compte.IdCompte, 0, soldeTemp, "R");
                    }
                    guichet.addTransaction(credit.IdCompte, 0, difference, "D");
                                                                                 
                    guichet.tblComptes.AddOrUpdate(compte);                    //S'assurer que les modifications ont été effectué dans la table comptes
                    guichet.tblComptes.AddOrUpdate(credit);
                                                                               //Enregistrer les changements
                    guichet.SaveChanges();
                }
                catch (Exception ex)
                {
                    lblErreur.Content = "Impossible d'effectuer un retrait pour le moment.";
                    Console.WriteLine(ex.Message);
                    return;
                }
                                                                              // Message de renseignement
                MessageBox.Show("Votre marge de crédit a augmenté de " + String.Format("{0:c}", difference) + ".", "Retrait", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else                                                        // Le solde est suffisant pour effectuer le retrait
            {
                try
                {
                    compte.Solde -= montant;                           // Soustraire le montant du retrait au solde du compte
                    argentpapier.ArgentPapier -= montant;              // Soustraire le montant du retrait à l'argent papier du guichet

                    guichet.addTransaction(compte.IdCompte, 0, montant, "R"); // Ajouter la transaction dans l'historique des transactions

                    guichet.tblComptes.AddOrUpdate(compte);                    //S'assurer que les modifications ont été effectué dans la table comptes
 
                    guichet.SaveChanges();                                    //Enregistrer les changements
                }
                catch (Exception ex)
                {
                    lblErreur.Content = "Impossible d'effectuer un retrait pour le moment.";
                    Console.WriteLine(ex.Message);
                    return;
                }
            }

                                                                            // Animation : Distributeur de billets 
            
            Panel.IsEnabled = false;                                        // Désactiver les controles de la fenetre pendant l'animation jusqu'à que le client prenne ces billets
            btnBack.IsEnabled = false;

            List<List<int>> combinations = FindCombinations((int)montant);  // Récupérer les combinaisons de billets possible

            if (combinations.Count() == 1)                                 // Si une seule combinaison est possible, lancer l'animation
            {
                animation(combinations[0]);
            }
            else                                                            // Si plusieurs combinaisons, afficher les combinaisons de billets possible 
            {
                Billets fen = new Billets(combinations, this, String.Format("{0:c}", montant));
                fen.ShowDialog();
            }
           
        }
        private void nouvFacture(decimal montant, tblCompte compte)   // Méthode pour payer une facture
        {
            MessageBoxResult btn;
            decimal frais = (decimal)1.25;

            if (compte.Solde < (montant + frais))                                      // Vérifier que le solde est suffisant                   
            {
                if (utilisateurActif.tblComptes.Where(c => c.Type == "M").Count() == 0) //Si le client ne posséde pas un compte de marge de crédit
                {                                                                      // Refuser la transaction
                    lblErreur.Content = compte.Aff + " ne contient pas assez de fond pour effectuer un paiement de " + String.Format("{0:c}", montant) + " plus 1,25$ de frais." ;
                    return;
                }
                                                                                  //Le client posséde une marge de crédit
                                                                                 // Demander au client s'il souhaite puiser dans son compte marge de crédit
                MessageBoxResult btns = MessageBox.Show(compte.Aff + " ne contient pas assez de fond pour effectuer un paiement de " + String.Format("{0:c}", (montant)) + " plus 1,25$ de frais.\r\nVoulez-vous puiser dans la marge de crédit?", "Payer une facture", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (btns != MessageBoxResult.Yes)                                   // Le client refuse      
                {
                    lblErreur.Content = "Paiement annulé.";
                    return;
                }

                                                                             // Le client souhaite puiser dans sa marge de crédit
                decimal soldeTemp = compte.Solde;
                decimal difference = (montant + frais) - soldeTemp;          // Calcule la différence                                                        
                                                                             // Récupérer le compte marge
                tblCompte credit = utilisateurActif.tblComptes.Where(c => c.Type == "M").First() as tblCompte;

                try
                { 
                    compte.Solde = 0;                                       // Solde du compte à 0
                    credit.Solde += difference;                             // Ajouter la différence au solde de la marge de crédit

                                                                            // Ajouter les transactions dans l'historique des transactions
                    if (soldeTemp != 0)
                    {
                        guichet.addTransaction(compte.IdCompte, 0, soldeTemp, "F");   
                    }

                    guichet.addTransaction(credit.IdCompte, 0, difference, "D");

                    guichet.tblComptes.AddOrUpdate(compte);                    //S'assurer que les modifications ont été effectué dans la table comptes
                    guichet.tblComptes.AddOrUpdate(credit);
                  
                    guichet.SaveChanges();                                     //Enregistrer les changements
                }
                catch (Exception ex)
                {
                    lblErreur.Content = "Impossible d'effectuer un paiement pour le moment.";
                    Console.WriteLine(ex.Message);
                    return;
                }
                                                                             // Message de confirmation
              btn =  MessageBox.Show("Vous avez payé une facture de " + String.Format("{0:c}", montant) + ", ainsi que des frais de 1,25$ avec les comptes " + compte.Aff + " et " + credit.Aff + ".\r\nVotre marge de crédit a augmenté de " + String.Format("{0:c}", difference) + ". \r\nDésirez-vous effectuer un autre paiement?", "Payer une facture", MessageBoxButton.YesNo, MessageBoxImage.Information);
            }
            else                                                           // Le solde est suffisant pour effectuer le paiement
            {
                try
                {
                    compte.Solde -= (montant + frais);                        // Soustraire le montant du paiement et les frais au solde du compte

                    guichet.addTransaction(compte.IdCompte, 0, montant, "F"); // Ajouter la transaction dans l'historique des transactions
                    guichet.addTransaction(compte.IdCompte, 0, frais, "P");

                    guichet.tblComptes.AddOrUpdate(compte);                    //S'assurer que les modifications ont été effectué dans la table comptes

                    guichet.SaveChanges();                                    //Enregistrer les changements
                }
                catch (Exception ex)
                {
                    lblErreur.Content = "Impossible d'effectuer un paiement pour le moment.";
                    Console.WriteLine(ex.Message);
                    return;
                }

                // Message de confirmation
               btn = MessageBox.Show("Vous avez payé une facture de " + String.Format("{0:c}", montant) + ", ainsi que des frais de 1,25$ avec le compte " + compte.Aff + ". \r\nDésirez-vous effectuer un autre paiement?", "Payer une facture", MessageBoxButton.YesNo, MessageBoxImage.Information);
            }

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

        private void btnConfirm_Click(object sender, RoutedEventArgs e)   // BOUTON CONFIRMER
        {
            lblErreur.Content = string.Empty;

            if (ComboCompte.SelectedIndex == -1                         // Vérifier que toutes les informations ont été saisies
                || txtMontant.Text.Trim() == string.Empty)
            {
                lblErreur.Content = "Veuillez saisir les informations demandées.";
                return;
            }

            tblCompte compte = ComboCompte.SelectedItem as tblCompte;        // Récupère le compte selectionné

            decimal montant;
            bool conversion = decimal.TryParse(txtMontant.Text, out montant); // Conversion du montant en decimal
            
            if (conversion && montant > 0)                                    // Si la conversion a réussi et montant est supérieur à 0
            {
                switch (fonction)                                             // Appeler la méthode correspondante à l'action que l'utilisateur veut faire
                {
                    case "D":
                        nouvDepot(montant, compte);
                        break;
                    case "R":
                        nouvRetrait(montant, compte);
                        break;
                    case "F":
                        nouvFacture(montant, compte);
                        break;
                }
            }
            else
            {
                lblErreur.Content = "Montant invalide";                     
                return;
            }
        }

        private void txtMontant_KeyDown(object sender, KeyEventArgs e)        // Evenement KeyDown sur le textbox du montant
        {
            if (e.Key == Key.Enter)                                           // Touche enter 
            {
                e.Handled = true;                                              // Empêche la touche "Enter" de produire son effet habituel 
                btnConfirm.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // Clique sur le bouton confirmer
            }
        }
    }
}

