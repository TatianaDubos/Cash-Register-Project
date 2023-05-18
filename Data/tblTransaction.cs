namespace GuichetAutomatique
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblTransaction")]
    public partial class tblTransaction
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCompteDepart { get; set; }

        public int? IdCompteDestination { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime Date { get; set; }

        [Key]
        [Column(Order = 2, TypeName = "smallmoney")]
        public decimal Montant { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(1)]
        public string Type { get; set; }

        public string nomType { get {
                string type = string.Empty;

                switch (Type)
                {
                    case "D":
                        type = "Dépôt";
                        break;
                    case "R":
                        type = "Retrait";
                        break;
                    case "T":
                        type = "Transfert";
                        break;
                    case "F":
                        type = "Facture";
                        break;
                    case "P":
                        type = "Frais";
                        break;
                    case "I":
                        type = "Intérêts";
                        break;
                    case "C":
                        type = "Remboursement";
                        break;
                }

                return type;
            } }

        public string Aff { get {
                string jour = string.Empty;
                string mois = string.Empty;
                string np = string.Empty;

                switch ((int)Date.DayOfWeek) 
                { 
                    case 1: jour = "Lundi";
                        break;
                    case 2: jour = "Mardi";
                        break;
                    case 3: jour = "Mercredi";
                        break;
                    case 4: jour = "Jeudi";
                        break;
                    case 5: jour = "Vendredi";
                        break;
                    case 6:  jour = "Samedi";
                        break;
                    case 7: jour = "Dimanche";
                        break;
                }

                switch ((int)Date.Month) 
                {
                    case 1: mois = "Janv.";
                        break;
                    case 2: mois = "Févr.";
                        break;
                    case 3: mois = "Mars";
                        break;
                    case 4: mois = "Avr.";
                        break;
                    case 5: mois = "Mai";
                        break;
                    case 6:  mois = "Juin";
                        break;
                    case 7: mois = "Juill.";
                        break;
                    case 8: mois = "Août";
                        break;
                    case 9: mois = "Sept.";
                        break;
                    case 10: mois = "Oct.";
                        break;
                    case 11: mois = "Nov.";
                        break;
                    case 12: mois = "Déc.";
                        break;
                }
                string date = jour + " " + Date.Day + " " + mois + " " + Date.Year + " " + Date.Hour + ":" + Date.Minute;

                switch (this.Type) 
                {
                    case "D":
                    case "I":
                        np = "+";
                        break;
                    case "P":
                    case "R" : 
                    case "F": np = "-";
                        break;
                  
                    
                }
             

                if(this.IdCompteDestination != null) 
                {
                    return "● " + nomType + "  -  " + date + "\r\n      ID: " + IdCompteDepart + ") Compte " + tblCompte.nomType + " → " + " ID: " + IdCompteDestination + ") Compte " + tblCompte1.nomType + "    "  + "⇄ " + String.Format("{0:c}", Montant) ;
                }
                else
                {
                    return "● " + nomType + "  -  " + date + "\r\n       ID: " + IdCompteDepart + ") Compte " + tblCompte.nomType + "     " + np + String.Format("{0:c}", Montant);
                }
        } }


        public virtual tblCompte tblCompte { get; set; }

        public virtual tblCompte tblCompte1 { get; set; }
    }
}
