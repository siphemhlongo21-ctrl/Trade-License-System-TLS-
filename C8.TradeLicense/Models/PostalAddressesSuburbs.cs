using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.Models
{
    public class PostalAddressesSuburbs : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "POSTAL CODE")]
        [StringLength(255)]
        public string POSTAL_CODE { get; set; }
        [Display(Name = "POSTAL AREA")]
        [StringLength(255)]
        public string POSTAL_AREA { get; set; }
        [Display(Name = "BOX TYPE")]
        [StringLength(255)]
        public string BOX_TYPE { get; set; }
        [Display(Name = "ORG UNIT")]
        [StringLength(255)]
        public string ORG_UNIT { get; set; }

        [Display(Name = "CITY")]
        [StringLength(255)]
        public string CITY { get; set; }


        [Display(Name = "RANGE FROM")]
        [StringLength(255)]
        public string RANGE_FROM { get; set; }

        [Display(Name = "RANGE TO")]
        [StringLength(255)]
        public string RANGE_TO { get; set; }

        [Display(Name = "LOGIN ROLE")]
        [StringLength(255)]
        public string LOGIN_ROLE { get; set; }

  
    }
}