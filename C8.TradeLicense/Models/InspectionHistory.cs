using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class InspectionHistory
    {
        [Key]
        public int InspectionHistoryId { get; set; }

        public InspectionHistory()
        {
            this.InspectionFailureCount = 0;
        }

        [Required]
        [ForeignKey("InspectionRequest")]
        [Display(Name = "Inspection Request")]
        public int InspectionRequestId { get; set; }
        public virtual InspectionRequest InspectionRequest { get; set; }

        public int InspectionResponseId { get; set; }

        [Display(Name = "Inspection Date")]
        public DateTime InspectionDateTime { get; set; }

        [Display(Name = "Inspection Failure Count")]
        public int InspectionFailureCount { get; set; }

        [Display(Name = "Non Compliance Date")]
        public DateTime? NonComplianceDateTime { get; set; }

        [Display(Name = "Compliance Date")]
        public DateTime? ComplianceDateTime { get; set; }

        [Display(Name = "Captured By Clerk")]
        public bool CapturedByClerk { get; set; }
        [Display(Name = " Clerk")]
        [StringLength(500)]
        public string Clerk { get; set; }

        [Display(Name = "Adhoc inspection")]
        public bool AdhocInspector { get; set; }

        [Required]
        [ForeignKey("License")]
        [Display(Name = "License")]
        public int LicenseId { get; set; }
        public virtual License License { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Required]
        [ForeignKey("Status")]
        [Display(Name = "Status")]
        public int StatusId { get; set; }
        public virtual Status Status { get; set; }
    }
}