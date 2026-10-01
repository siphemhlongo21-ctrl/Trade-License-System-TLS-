using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C8.TradeLicense.Models
{
    public class Document : BaseModel
    {
        [Key]
        public int DocumentId { get; set; }

        [Display(Name = "Document")]
        [Required(ErrorMessage = "Document Name Required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Document Name must be between 3 and 100 characters!")]
        public string DocumentName { get; set; }

        [Display(Name = "Document Description")]
        public string DocumentDescription { get; set; }

        [Display(Name = "Document Key")]
        [Required(ErrorMessage = "Document Key Required")]
        public string DocumentKey { get; set; }

        public int? DocumentTypeId { get; set; }
        [ForeignKey("DocumentTypeId")]
     
        public DocumentType DocumentType { get; set; }
    }
}