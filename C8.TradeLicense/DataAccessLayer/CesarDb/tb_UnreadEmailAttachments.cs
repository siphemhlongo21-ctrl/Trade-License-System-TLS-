namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_UnreadEmailAttachments
    {
        [Key]
        public int EmailAttachmentId { get; set; }

        public int EmailItemId { get; set; }

        [StringLength(1024)]
        public string Filename { get; set; }

        [StringLength(50)]
        public string ContentType { get; set; }

        [Required]
        public byte[] Attachment { get; set; }

        public bool IsProcessed { get; set; }
    }
}
