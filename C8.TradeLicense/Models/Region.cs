using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C8.TradeLicense.Models
{
    public class Region : BaseModel
    {
        public int RegionId { get; set; }

        [Display(Name = "Region")]
        [Required(ErrorMessage = "Region Name Required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Region Name must be between 3 and 100 characters!")]
        public string RegionName { get; set; }

        [Required(ErrorMessage = "Region Key Required")]
        [Display(Name = "Region Key")]
        public string RegionKey { get; set; }
    }
}