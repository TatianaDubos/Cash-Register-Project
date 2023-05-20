using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace GuichetAutomatique
{
    internal class Clavier
    {
        public static void aff(object obj, string lettre) {  // Méthode pour écrire les numéros
                                                             // saisis dans le clavier numérique à l'écran
            
            if (obj.GetType() == typeof(TextBox))  // Si l'objet est un TEXBOX
            {
                
                TextBox text = (TextBox)obj;

                if (text.MaxLength == text.Text.Length) return; // Quitter si la longueur maximum a été atteinte
                 
                text.Text += lettre; // Ajouter la lettre

                text.Select(text.Text.Length, 0); // Déplacer le curseur

            }
            else if (obj.GetType() == typeof(PasswordBox)) // Si l'objet est PASSWORDBOX
            {
                PasswordBox password = (PasswordBox)obj;

                if (password.MaxLength == password.Password.Length) return; // Quitter si la longueur maximum a été atteinte
               
                password.Password += lettre; // Ajouter la lettre

                password.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(password, new object[] { password.Password.Length, 0 }); // Déplacer le curseur
            }
            
          
        }
    }
}
