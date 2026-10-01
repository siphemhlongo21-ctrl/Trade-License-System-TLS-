using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using C8.TradeLicense.DataAccessLayer;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class Department : BaseModel
    {
        [Key]
        public int DepartmentId { get; set; }

        [Display(Name = "Department Name")]
        [Required]
        //[RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage="The Department Name field should consist of characters only")]
        public string DepartmentName { get; set; }

        [Display(Name = "Department Description")]
        [Required]
        //[RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "The Department Description field should consist of characters only")]
        public string DepartmentDescription { get; set; }

        [Display(Name = "Department Key")]
        [Required]
        public string DepartmentKey { get; set; }

        [Display(Name = "Department Structure")]
        [Required]
        public string DepartmentStructureType { get; set; }

        [Display(Name = "Region")]
        [Required]
        public int? RegionId { get; set; }
        [ForeignKey("RegionId")]
        public Region Region { get; set; }
    }
}