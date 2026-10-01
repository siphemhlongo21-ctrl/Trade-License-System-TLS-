namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_EmailQueueProcessed
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmailQueueId { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime QueueDateTime { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ApplicationId { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmailAccountId { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(512)]
        public string ToList { get; set; }

        [StringLength(512)]
        public string CcList { get; set; }

        [StringLength(512)]
        public string BccList { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(1024)]
        public string Subject { get; set; }

        [Key]
        [Column(Order = 6)]
        public string Body { get; set; }

        [Key]
        [Column(Order = 7)]
        public bool IsHtml { get; set; }

        [Key]
        [Column(Order = 8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FailureCount { get; set; }

        [StringLength(32)]
        public string ReferenceId { get; set; }

        public int? ReferenceTypeId { get; set; }

        [Key]
        [Column(Order = 9)]
        public DateTime ProcessedDateTime { get; set; }

        [Key]
        [Column(Order = 10)]
        public bool HasAttachments { get; set; }
    }
}
