using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class Amendment: BaseModel
    {
        [Key]
        public int AmendmentsId { get; set; }

        public int LicenseId { get; set; }
        [ForeignKey("LicenseId")]
        public License License { get; set; }

        [Display(Name = "Amendment Type")]
        [StringLength(50)]
        public string AmendmentType { get; set; }

        [Display(Name = "Table Amended")]
        [StringLength(80)]
        public string TableAmended { get; set; }

        [Display(Name = "Field Amended")]
        [StringLength(100)]
        public string FieldAmended { get; set; }

        [Display(Name = "Old Data")]
        [StringLength(100)]
        public string OldData { get; set; }

        [Display(Name = "New Data")]
        [StringLength(100)]
        public string NewData { get; set; }


    }

    public class AmendmentEnitity
    {
      
        public Amendment Amendment { get; set; }
        public object Entity { get; set; }
    }
}