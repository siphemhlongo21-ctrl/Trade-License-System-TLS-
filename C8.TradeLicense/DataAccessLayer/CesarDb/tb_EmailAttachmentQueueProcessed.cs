namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_EmailAttachmentQueueProcessed
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmailAttachmentQueueId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmailQueueId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ApplicationId { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(1024)]
        public string Filename { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(50)]
        public string ContentType { get; set; }

        [Key]
        [Column(Order = 5)]
        public byte[] Attachment { get; set; }
    }
}
