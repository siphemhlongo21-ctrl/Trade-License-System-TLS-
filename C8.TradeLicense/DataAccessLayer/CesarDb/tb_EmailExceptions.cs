namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_EmailExceptions
    {
        [Key]
        public int EmailExceptionId { get; set; }

        public int EmailExceptionTypeId { get; set; }

        public int EmailQueueId { get; set; }

        [StringLength(512)]
        public string ToList { get; set; }

        public string Exception { get; set; }

        public DateTime ProcessedDateTime { get; set; }

        public int AttemptCount { get; set; }
    }
}
