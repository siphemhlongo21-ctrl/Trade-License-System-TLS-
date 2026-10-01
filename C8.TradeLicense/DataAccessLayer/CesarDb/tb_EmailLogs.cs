namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_EmailLogs
    {
        [Key]
        public int EmailLogID { get; set; }

        public int ApplicationID { get; set; }

        public int? EmailTemplateID { get; set; }

        [Required]
        [StringLength(255)]
        public string EmailTo { get; set; }

        [Required]
        [StringLength(255)]
        public string EmailFrom { get; set; }

        [Required]
        [StringLength(255)]
        public string EmailSubject { get; set; }

        [Required]
        public string EmailBody { get; set; }

        public DateTime SentDateTime { get; set; }

        public int SentByUserID { get; set; }
    }
}
