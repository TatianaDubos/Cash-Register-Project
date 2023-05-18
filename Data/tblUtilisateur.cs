namespace GuichetAutomatique
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblUtilisateur")]
    public partial class tblUtilisateur
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblUtilisateur()
        {
            tblComptes = new HashSet<tblCompte>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(15)]
        public string Prenom { get; set; }

        [Required]
        [StringLength(15)]
        public string Nom { get; set; }

        [Required]
        [StringLength(13)]
        public string Telephone { get; set; }

        [Required]
        [StringLength(30)]
        public string Courriel { get; set; }

        [Required]
        [StringLength(4)]
        public string NIP { get; set; }

        public bool Acces { get; set; }

        public bool Admin { get 
            {
                if (this.ID == 1) return (true);
                else return (false);    
            } 
        }

        public string Aff
        {
            get
            {
                return this.ID + ") " + this.Prenom + " " + this.Nom;
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCompte> tblComptes { get; set; }
    }
}
