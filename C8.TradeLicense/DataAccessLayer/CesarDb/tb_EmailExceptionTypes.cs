namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_EmailExceptionTypes
    {
        [Key]
        public int EmailExceptionTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string EmailExceptionTypeName { get; set; }

        [StringLength(256)]
        public string EmailExceptionTypeDescription { get; set; }

        [Required]
        [StringLength(50)]
        public string EmailExceptionTypeKey { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
