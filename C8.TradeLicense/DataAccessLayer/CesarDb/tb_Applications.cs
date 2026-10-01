namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Applications
    {
        [Key]
        public int ApplicationID { get; set; }

        [Required]
        [StringLength(50)]
        public string ApplicationKey { get; set; }

        [Required]
        [StringLength(100)]
        public string ApplicationName { get; set; }

        [StringLength(250)]
        public string ApplicationDescription { get; set; }

        [StringLength(250)]
        public string ApplicationURL { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
