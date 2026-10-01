using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class LookupTable
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Status Name")]
        [StringLength(200)]
        public string StatusName { get; set; }

        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public Status Status { get; set; }

        [Display(Name = "Escalation Day")]
        public int EscalationDay { get; set; }

        public string RoleId { get; set; }
        [ForeignKey("RoleId")]
        public AspNetRole AspNetRole { get; set; }

        public string RoleName { get; set; }
    }
}