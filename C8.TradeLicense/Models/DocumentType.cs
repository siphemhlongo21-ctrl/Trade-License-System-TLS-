using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace C8.TradeLicense.Models
{
    public class DocumentType : BaseModel
    {
        [Key]
        public int DocumentTypeId { get; set; }

        [Display(Name = "Document Type Name")]
        [Required(ErrorMessage = "Document Type Required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Document Type must be between 3 and 100 characters!")]
        public string DocumentTypeName { get; set; }

        [Display(Name = "Document Type Description")]
        public string DocumentTypeDescription { get; set; }

        [Display(Name = "Document Type Key")]
        [Required(ErrorMessage = "Document Type Key Required")]
        public string DocumentTypeKey { get; set; }

      
    
    }
}