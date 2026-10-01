namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_EmailAttachmentQueue
    {
        [Key]
        public int EmailAttachmentQueueId { get; set; }

        public int EmailQueueId { get; set; }

        public int ApplicationId { get; set; }

        [Required]
        [StringLength(1024)]
        public string Filename { get; set; }

        [Required]
        [StringLength(50)]
        public string ContentType { get; set; }

        [Required]
        public byte[] Attachment { get; set; }
    }
}
