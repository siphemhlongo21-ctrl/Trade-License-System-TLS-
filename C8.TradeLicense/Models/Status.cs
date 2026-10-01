using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using C8.TradeLicense.Models;

namespace C8.TradeLicense.Models
{
    public class Status : BaseModel
    {
        [Key]
        public int StatusId { get; set; }

        [Required(ErrorMessage = "Please enter Status Name.")]
        [Display(Name = "Status")]
        public string StatusName { get; set; }

        [Required(ErrorMessage = "Please select Status Type.")]
        public int StatusTypeId { get; set; }
        [ForeignKey("StatusTypeId")]
        public StatusType StatusType { get; set; }

        public string StatusDescription { get; set; }

        [Required(ErrorMessage = "Please enter Status Key.")]
        public string StatusKey { get; set; }
    }
}