using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.Models
{
    public class NumOfDays : BaseModel
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Step Name")]
        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "NumberOfDays")]
        public int NumberOfDays { get; set; }
    }
}