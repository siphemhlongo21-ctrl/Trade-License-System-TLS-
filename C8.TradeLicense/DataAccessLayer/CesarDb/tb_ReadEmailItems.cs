namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_ReadEmailItems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmailItemId { get; set; }

        public int EmailAccountId { get; set; }

        public int ApplicationId { get; set; }

        public int? EmailExceptionTypeId { get; set; }

        public int? ReferenceTypeId { get; set; }

        public int? ReferenceId { get; set; }

        [Required]
        public string FromList { get; set; }

        [Required]
        public string ToList { get; set; }

        public string CcList { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool? IsHtml { get; set; }

        public DateTime RecievedDateTime { get; set; }

        public bool HasAttachments { get; set; }
    }
}
