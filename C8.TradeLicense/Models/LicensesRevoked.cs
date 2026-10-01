using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class LicensesRevoked:BaseModel
    {
        [Key]
        public int LicensesRevokedId { get; set; }

        [Required]
        [ForeignKey("License")]
        [Display(Name = "License")]
        public int LicenseId { get; set; }
        public virtual License License { get; set; }

        [Display(Name = "Decision")]
        [StringLength(50)]
        public string Decision { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "PreviousStatusId")]
        public int PreviousStatusId { get; set; }
        [ForeignKey("PreviousStatusId")]
        public Status Status { get; set; }

        [Display(Name = " Decision Date")]
        public DateTime? DecisionDate { get; set; }
    }
}