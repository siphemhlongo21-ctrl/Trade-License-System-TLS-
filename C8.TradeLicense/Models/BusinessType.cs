using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class BusinessType : BaseModel
    {
        [Key]
        public int BusinessTypeId { get; set; }
        [Display( Name = "Business Type" )]
        [Required(ErrorMessage = "Business Type must be between 3 and 100 characters!")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Business Type must be between 3 and 100 characters!")]
        public string BusinessTypeName { get; set; }
        [Display( Name = "Business Type Description" )]
        [Required(ErrorMessage = "Business Type Description must be between 3 and 100 characters!")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Business Type Description must be between 3 and 100 characters!")]
        public string BusinessTypeDescription { get; set; }
    }
}