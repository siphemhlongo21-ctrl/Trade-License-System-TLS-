namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_EmailTemplates
    {
        [Key]
        public int EmailTemplateID { get; set; }

        public int ApplicationID { get; set; }

        [Required]
        [StringLength(50)]
        public string EmailTemplateName { get; set; }

        [StringLength(50)]
        public string EmailTemplateDescription { get; set; }

        [Required]
        [StringLength(50)]
        public string EmailTemplateKey { get; set; }

        [Required]
        [StringLength(50)]
        public string EmailFrom { get; set; }

        [Required]
        [StringLength(50)]
        public string EmailSubject { get; set; }

        [Required]
        public string EmailBody { get; set; }

        public string Variables { get; set; }

        public bool IsHTML { get; set; }

        public string ToVariable { get; set; }

        public string CcVariable { get; set; }

        public bool HasVariables { get; set; }

        public bool? SendAsOneEmail { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
