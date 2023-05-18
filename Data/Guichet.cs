using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace GuichetAutomatique
{
    public partial class Guichet : DbContext
    {
        public Guichet()
            : base("name=Guichet")
        {
        }

        public virtual DbSet<tblCompte> tblComptes { get; set; }
        public virtual DbSet<tblUtilisateur> tblUtilisateurs { get; set; }
        public virtual DbSet<tblGuichet> tblGuichets { get; set; }
        public virtual DbSet<tblTransaction> tblTransactions { get; set; }

        public void addTransaction(int compteDepart, int compteDestination, decimal montant, string type)  // Ajouter une transaction
        {
            tblTransaction newTran = new tblTransaction();

            try
            {
                newTran.Date = DateTime.Now;

                newTran.IdCompteDepart = compteDepart;

                if (compteDestination != 0)
                {
                    newTran.IdCompteDestination = compteDestination;
                }
                else 
                {
                    newTran.IdCompteDestination = null;
                }

                newTran.Montant = montant;
                newTran.Type = type;

                this.tblTransactions.Add(newTran);
                this.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            } 
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblCompte>()
                .Property(e => e.Type)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblCompte>()
                .Property(e => e.Solde)
                .HasPrecision(19, 4);

            modelBuilder.Entity<tblCompte>()
                .HasMany(e => e.tblTransactions)
                .WithRequired(e => e.tblCompte)
                .HasForeignKey(e => e.IdCompteDepart)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblCompte>()
                .HasMany(e => e.tblTransactions1)
                .WithOptional(e => e.tblCompte1)
                .HasForeignKey(e => e.IdCompteDestination);

            modelBuilder.Entity<tblUtilisateur>()
                .Property(e => e.Telephone)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUtilisateur>()
                .Property(e => e.NIP)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUtilisateur>()
                .HasMany(e => e.tblComptes)
                .WithRequired(e => e.tblUtilisateur)
                .HasForeignKey(e => e.IdClient);

            modelBuilder.Entity<tblGuichet>()
                .Property(e => e.ArgentPapier)
                .HasPrecision(10, 4);

            modelBuilder.Entity<tblTransaction>()
                .Property(e => e.Montant)
                .HasPrecision(10, 4);

            modelBuilder.Entity<tblTransaction>()
                .Property(e => e.Type)
                .IsFixedLength()
                .IsUnicode(false);
        }
    }
}
