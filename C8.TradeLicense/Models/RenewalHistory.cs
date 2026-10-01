using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class RenewalHistory : BaseModel
    {

        [Key]
        public int RenewalHistoryId { get; set; }

        [Display(Name = "Old Renewal Date")]
        public DateTime? OldRenewalDateTime { get; set; }
       [Display(Name = "New Renewal Date")]
        public DateTime? NewRenewalDateTime { get; set; }


        public int LicenseId { get; set; }
        [ForeignKey("LicenseId")]
        public License License{ get; set; }
    }
}