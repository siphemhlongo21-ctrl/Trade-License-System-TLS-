using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using C8.TradeLicense.Models;

namespace C8.TradeLicense.Models
{
    public class MigratedLicense : BaseModel
    {
        [Key]
        public int MLicenseId { get; set; }



        [Display(Name = "Type of License")]

        public int? LicenseTypeId { get; set; }
        [ForeignKey("LicenseTypeId")]
        public LicenseType LicenseType { get; set; }

        public int? MClientId { get; set; }
        [ForeignKey("MClientId")]
        public MigratedClient MigratedClient { get; set; }

        public int? ItemTypeId { get; set; }
        [ForeignKey("ItemTypeId")]
        public ItemType ItemType { get; set; }

        [Display(Name = "Sub-Category")]
        
        public int? ItemSubCategoryId { get; set; }
        [ForeignKey("ItemSubCategoryId")]     
        public ItemSubCategory ItemSubCategory { get; set; }

        public int? MBusinessId { get; set; }
        [ForeignKey("MBusinessId")]
        public MigratedBusiness MigratedBusiness { get; set; }

        [Display(Name = "License Number")]
        [StringLength(20)]
        public string LicenseNumber { get; set; }

        [Display(Name = "Region")]
   
        public int? RegionId { get; set; }
        [ForeignKey("RegionId")]    
        public Region Region {get; set;}

        [Display(Name = "Licence Issue Year")]
        public int? LicenseIssueYear { get; set; }

        [Display(Name = "Date of Application")]
        public DateTime? ApplicationDateTime { get; set; }

        [Display(Name = "License Issue Date")]
        public DateTime? LicenseIssueDateTime { get; set; }

        [Display(Name = "Notification Updated Date")]
        public DateTime? NotificationUpdatedDateTime { get; set; }

        [Display(Name = "License Expiry Date")]
        public DateTime? LicenseExpiryDate { get; set; }

        public int? StatusId { get; set; }
        [ForeignKey("StatusId")]
        public Status Status { get; set; }
        //public Business Business { get; set; } 

        [Display(Name = "Licence Closure Date")]
        public DateTime? LicenseClosureDateTime { get; set; }

        [Display(Name = "Licence Collection Date")]
        public DateTime? LicenseCollectedDateTime { get; set; }

        [Display(Name = "Licence Renewal Date")]
        public DateTime? LicenseRenewalDateTime { get; set; }

        [Display(Name = "Licence Conditions")]
        public int? ItemConditionId { get; set; }
        [ForeignKey("ItemConditionId")]
        public ItemCondition ItemCondition { get; set; }

        [Display(Name = "Condition Change Reason")]
      
        public string ConditionReason { get; set; }

        [Display(Name = "Condition Change Comment")]
       
        public string ConditionComment { get; set; }

        internal object Include(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
    }
}
