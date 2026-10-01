using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class LicenseType : BaseModel
    {
        [Key]
        public int LicenseTypeId { get; set; }

        [Display(Name = "Type of License")]
        [Required(ErrorMessage = "License Type Required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "License Type must be between 3 and 100 characters!")]
        public string LicenseTypeName { get; set; }

        [Required(ErrorMessage = "License Type Key Required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "License Type Key must be between 3 and 100 characters!")]
        [Display(Name = "Key")]
        public string LicenseTypeKey { get; set; }

        [Display(Name = "Description")]
        public string LicenseTypeDescription { get; set; }

        [Display(Name="Application Fee")]
        public decimal Amount { get; set; }
    }
}