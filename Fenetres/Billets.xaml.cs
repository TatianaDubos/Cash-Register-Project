using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
    /// Logique d'interaction pour Billets.xaml
    /// </summary>
    public partial class Billets : Window
    {
        List<List<int>> combinations;
        Transaction fen;
        bool close = true;  // Fermeture de la fenetre non autorisé
        public Billets(List<List<int>> combinations, Transaction fen, string montant) // FENETRE POUR CHOISIR LE NOMBRE DE BILLETS VOULU
        {
            InitializeComponent();
          
            this.combinations = combinations;
            this.fen = fen;

            lblMontant.Content = "(Pour un total de " + montant + ")";      // Afficher le montant
            btn3.Visibility = Visibility.Hidden;                            // Le 3e bouton est facultatif, on rend invisible pour l'instant

            for(int i = 1; i <= combinations.Count; i++)                    // Determiner les billets qui forment chaque combinaison
            {
                List<int> comb = combinations[i-1];

                int bill10 = 0;
                int bill20 = 0;
                int bill50 = 0;
                int bill100 = 0;

                foreach (int num in comb)                                // Compte le nombre de billets de 10, 20,50 ou 100 
                {
                    switch (num)                                          
                    { 
                        case 10: bill10++;
                            break;
                        case 20: bill20++;
                            break;
                        case 50: bill50++;
                            break;
                        case 100: bill100++;
                            break;
                    }

                }

                Button btn = new Button();

                switch (i)                  // Récupérer le bouton voulu selon le compteur de la boucle
                {
                    case 1:
                        btn = btn1;          // Bouton qui affiche le 1e choix
                        break;
                    case 2:
                        btn = btn2;         // Bouton qui affiche le 2e choix
                        break;
                    case 3:
                        btn = btn3;         // Bouton qui affiche le 3e choix
                        btn3.Visibility=Visibility.Visible;    
                        break;
                   
                }
                                         // Afficher la combinaison dans le bouton
                if (bill10 != 0) btn.Content += bill10 + " billets de 10$\r\n";
               if (bill20 != 0) btn.Content += bill20 + " billets de 20$\r\n";
                if (bill50 != 0) btn.Content += bill50 + " billets de 50$\r\n";
                if (bill100 != 0) btn.Content += bill100 + " billets de 100$\r\n";
            }

        }

        private void btn_Click(object sender, RoutedEventArgs e)  // Evenement click sur un bouton
        {
            Button btn = sender as Button;                      // Récupérer le bouton
          
            if (btn == btn1) fen.animation(combinations[0]);    // Lancer l'animation du distributeur de billet avec la combinaison voulu en parametre
            else if (btn == btn2) fen.animation(combinations[1]);
            else if (btn == btn3) fen.animation(combinations[2]);

            close = false;                                      // Fermeture de la fenetre autorisé
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) 
        {
            e.Cancel = close;                                   // Empecher de fermer la fenetre si aucun bouton a été choisi
        }
    }
}
