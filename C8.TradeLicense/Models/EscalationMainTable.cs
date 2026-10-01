using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class EscalationMainTable
    {
        [Key]
        public int ID { get; set; }

        public int LicenseId { get; set; }
        [ForeignKey("LicenseId")]
        public License License { get; set; }

        [Display(Name = "StatusDaysOLD")]
        public int StatusDaysOLD { get; set; }

        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public Status Status { get; set; }


    }
}