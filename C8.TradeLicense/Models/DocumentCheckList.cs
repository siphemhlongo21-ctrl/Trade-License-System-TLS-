using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class DocumentCheckList : BaseModel
    {

        [Key]
        public int DocumentCheckListId { get; set; }

        [Required(ErrorMessage = "License Type Required")]
        [ForeignKey("LicenseType")]
        [Display(Name = "License Type")]
        public int LicenseTypeId { get; set; }
        public virtual LicenseType LicenseType { get; set; }

        [Required(ErrorMessage = "Document Required")]
        [ForeignKey("Document")]
        [Display(Name = "Document")]
        public int DocumentId { get; set; }
        public virtual Document Document { get; set; }
        
        //public LicenseType LicenseTypeDetails { get; set; } 
        //public DocumentType DocumentTypeDetails { get; set; }


    }
}