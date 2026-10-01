namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_ReferenceTypes
    {
        [Key]
        public int ReferenceTypeId { get; set; }

        [Required]
        [StringLength(512)]
        public string ReferenceTypeName { get; set; }

        [StringLength(1024)]
        public string ReferenceTypeDescription { get; set; }

        [Required]
        [StringLength(256)]
        public string ReferenceTypeKey { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsLocked { get; set; }
    }
}
