using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Logique d'interaction pour AccueilAdministrateur.xaml
    /// </summary>
    public partial class AccueilAdministrateur : Window  // FENETRE D'ACCUEIL POUR ADMINISTRATEUR
    {
        tblUtilisateur utilisateurActif;
        public AccueilAdministrateur(tblUtilisateur utilisateur)
        {
            InitializeComponent();
            this.utilisateurActif = utilisateur;

            lblNom.Content += utilisateurActif.Prenom + " " + utilisateurActif.Nom;   // Afficher le nom de l'utilisateur
        }

        private void btnClient_Click(object sender, RoutedEventArgs e) // NOUVEAU CLIENT
        {
            CreerClient fen = new CreerClient(utilisateurActif);
            fen.Show();
            this.Close();
        }

        private void btnCompte_Click(object sender, RoutedEventArgs e) // NOUVEAU COMPTE
        {
            CreerCompte creerCompte = new CreerCompte(utilisateurActif);
            creerCompte.Show();
            this.Close();
        }

        private void btnTransaction_Click(object sender, RoutedEventArgs e) // CONSULTER LES TRANSACTIONS
        {
            ChoisirCompte fen = new ChoisirCompte(utilisateurActif);
            fen.Show();
            this.Close();
        }

        private void btnAcces_Click(object sender, RoutedEventArgs e) // MODIFIER L'ACCES
        {
            Acces fen = new Acces(utilisateurActif);    
            fen.Show();
            this.Close();
        }

        private void btnArgent_Click(object sender, RoutedEventArgs e) // AJOUTER DE L'ARGENT PAPIER
        {
            ArgentPapier fen = new ArgentPapier(utilisateurActif);
            fen.Show();
            this.Close();
        }

     

        private void btnInteret_Click(object sender, RoutedEventArgs e) // PAYER LES INTERETS
        {
            // Message pour demander confirmation
            MessageBoxResult btn = MessageBox.Show("Voulez-vous réellement payer 1% d'intérêt à tous les comptes épargne?", "Intérêts", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (btn != MessageBoxResult.Yes ) return;  

            Guichet guichet = new Guichet();
            decimal interet;

            try 
            {
                guichet.tblComptes.Where(c => c.Type == "E").ToList().ForEach(c => { // Augmenter de 1% le solde des comptes épargne
                    
                     interet = (decimal)0.01 * c.Solde;                             // Calcul du montant
                    
                    if(interet > 0) 
                    {
                        c.Solde += interet;                                         // Ajout du montant au solde
                        guichet.addTransaction(c.IdCompte, 0, interet, "I");        // Ajouter une transaction de type Intérêts
                    }
                   
                }) ;

                guichet.SaveChanges();                                             // Enregistrer les changements dans la base de données
            }
            catch (Exception ex) 
            {
               Console.WriteLine(ex.Message);
               MessageBox.Show("Impossible d'effectuer cette action pour le moment.", "Intérêts", MessageBoxButton.OK, MessageBoxImage.Error);
               return; 
            }

            // Message de confirmation
            MessageBox.Show("Les intérêts ont été payés avec succès.", "Intérêts", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnFermer_Click(object sender, RoutedEventArgs e)  // FERMER LE GUICHET
        {
            //Message de confirmation
            if (MessageBox.Show("Voulez-vous réellement fermer le guichet?", "Fermeture", MessageBoxButton.YesNo, MessageBoxImage.Question)
               == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btnQuitter_Click(object sender, RoutedEventArgs e) // BOUTON QUITTER
        {
            Login fen = new Login();
            fen.Show();
            this.Close();
        }
    }
}
