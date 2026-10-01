using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using C8.TradeLicense.Models;

namespace C8.TradeLicense.Models
{
    public class StatusType : BaseModel
    {
        [Key]
        public int StatusTypeId { get; set; }
        [Required(ErrorMessage = "Status Type Name Required")]
        [Display(Name = "Status Type Name")]
        public string StatusTypeName { get; set; }
        [Display(Name = "Description")]
        public string StatusTypeDescription { get; set; }
    }
}