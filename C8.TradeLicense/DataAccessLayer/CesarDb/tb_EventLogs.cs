namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_EventLogs
    {
        [Key]
        public int EventLogID { get; set; }

        public int? ApplicationID { get; set; }

        public int? UserID { get; set; }

        public DateTime EventDateTime { get; set; }

        [Required]
        public string EventData { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
