namespace GuichetAutomatique
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Data.SqlTypes;

    [Table("tblGuichet")]
    public partial class tblGuichet
    {
        [Key]
        [Column(TypeName = "tinyint")]
        public byte ID { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal ArgentPapier { get; set; }
    }
}
