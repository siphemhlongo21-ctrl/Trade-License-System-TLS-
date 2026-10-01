using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.Models
{
    public class PublicHolidays: BaseModel
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Date")]
        [StringLength(50)]
        public string Date { get; set; }

        [Display(Name = "Name")]
        [StringLength(150)]
        public string Name { get; set; }
    }
}