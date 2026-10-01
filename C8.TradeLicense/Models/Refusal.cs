using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C8.TradeLicense.Models 
{
    public class Refusal : BaseModel
    {

        [Key]
        public int RefusalId { get; set; }

        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public Status Status { get; set; }

        [Required]
        [ForeignKey("License")]
        [Display(Name = "License")]
        public int LicenseId { get; set; }
        public virtual License License { get; set; }

        [Required]
        [ForeignKey("InspectionResponse")]
        [Display(Name = "InspectionResponseId")]
        public int InspectionResponseId { get; set; }
        public virtual InspectionResponse InspectionResponse { get; set; }

        [Required]
        [ForeignKey("InspectionRequest")]
        [Display(Name = "Inspection Request")]
        public int InspectionRequestId { get; set; }
        public virtual InspectionRequest InspectionRequest { get; set; }

        [Display(Name = "Upload Date")]
        public DateTime? DateLog { get; set; }

      

    }
}