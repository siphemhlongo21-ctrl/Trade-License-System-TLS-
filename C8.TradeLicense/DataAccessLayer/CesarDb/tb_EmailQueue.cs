namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_EmailQueue
    {
        [Key]
        public int EmailQueueId { get; set; }

        public DateTime QueueDateTime { get; set; }

        public int ApplicationId { get; set; }

        public int EmailAccountId { get; set; }

        [Required]
        [StringLength(512)]
        public string ToList { get; set; }

        [StringLength(512)]
        public string CcList { get; set; }

        [StringLength(512)]
        public string BccList { get; set; }

        [Required]
        [StringLength(1024)]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        public bool IsHtml { get; set; }

        public int FailureCount { get; set; }

        [StringLength(32)]
        public string ReferenceId { get; set; }

        public int? ReferenceTypeId { get; set; }

        public bool HasAttachments { get; set; }
    }
}
