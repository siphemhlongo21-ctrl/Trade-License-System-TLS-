using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C8.TradeLicense.Models
{
    public class InspectionRequest : BaseModel
    {
        [Key]
        public int InspectionRequestId { get; set; }
        
        
        [Display(Name = "Override Order Index")]
        public bool OverrideOrderIndex { get; set; }

        public int LicenseId { get; set; }
        [ForeignKey("LicenseId")]
        public License License { get; set; }

        [Display(Name = "Inspector")]
              public int DepartmentContactId { get; set; }

     
      


        [StringLength(128)]
        [Display(Name = "Reference Number")]
        public string RefNumber { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }


        //[DisplayFormat(DataFormatString = "{dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Allocated Date")]
        public DateTime AllocatedDate { get; set; }

     
        public int LicenseTypeId { get; set; }
        [ForeignKey("LicenseTypeId")]
        public LicenseType LicenseType { get; set; }

       

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }

        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public Status Status { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime SlaExpiryDate { get; set; }

        [Display(Name = "Final Date")]
        public DateTime? AppealFinalDate { get; set; }

        [Display(Name = "Inspection Final Date")]
        public DateTime? InspectionFinalDate { get; set; }

    }
}