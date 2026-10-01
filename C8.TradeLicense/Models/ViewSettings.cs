using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class ViewSettings:BaseModel
    {
        [Key]
        public int ViewSettingsId { get; set; }

        [Display(Name = "Name of function")]

       
        public string ItemName { get; set; }

        


        public string ItemKey { get; set; }

        [Display(Name = "User Role")]
     
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Role must be between 3 and 50 characters!")]
        public string Role { get; set; }

        [Display(Name = "Description")]
       
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Role must be between 3 and 255 characters!")]

        public string Description { get; set; }

    }
}