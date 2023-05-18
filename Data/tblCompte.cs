namespace GuichetAutomatique
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblCompte")]
    public partial class tblCompte
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblCompte()
        {
            tblTransactions = new HashSet<tblTransaction>();
            tblTransactions1 = new HashSet<tblTransaction>();
        }

        [Key]
        public int IdCompte { get; set; }

        public int IdClient { get; set; }

        [Required]
        [StringLength(1)]
        public string Type { get; set; }

        public string nomType { get {
                string nom = string.Empty;   
                    switch (this.Type)
                {
                    case "C":
                         nom = "Chèque";
                        break;
                    case "E":
                        nom =  "Épargne";
                        break;
                    case "H":
                        nom = "Hypothécaire";
                        break;
                    case "M":
                        nom = "Marge de crédit";
                        break;
                }

                return nom;
            } }

        [Column(TypeName = "money")]
        public decimal Solde { get; set; }

        public string Aff { get {
                return "ID: " + this.IdCompte.ToString() + ") Compte " + nomType ;
         } }

        public string AffAdmin
        {
            get
            {
                return  "ID: " + this.IdCompte.ToString() + ") Compte " + nomType + " - " + this.tblUtilisateur.Nom + ", " + tblUtilisateur.Prenom;
            }
        }

        public virtual tblUtilisateur tblUtilisateur { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblTransaction> tblTransactions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblTransaction> tblTransactions1 { get; set; }
    }
}
