namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_AppSettings
    {
        [Key]
        public int AppSettingID { get; set; }

        public int ApplicationID { get; set; }

        [Required]
        [StringLength(256)]
        public string AppKey { get; set; }

        [Required]
        [StringLength(256)]
        public string AppValue { get; set; }

        [Required]
        [StringLength(256)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
