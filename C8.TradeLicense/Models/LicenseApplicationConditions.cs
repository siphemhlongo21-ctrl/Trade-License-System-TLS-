using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.Models
{
    public class LicenseApplicationConditions : BaseModel
    {
        [Key]
        public int LicenseApplicationConditionsId { get; set; }
        public int LicenseId { get; set; }
        [ForeignKey("LicenseId")]
        public License License { get; set; }
        public int ConditionId { get; set; }
        [ForeignKey("ConditionId")]
        public Condition Condition { get; set; }

        [Display(Name = "Condition Name")]
   
        public string ConditionName { get; set; }

        [Display(Name = "selected")]
        public bool IsSelected { get; set; }
    }
}