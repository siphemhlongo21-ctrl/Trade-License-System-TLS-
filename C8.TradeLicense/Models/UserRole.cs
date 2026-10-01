using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class UserRole
    {
        [Key]
        public int UserRoleId { get; set; }

        [Display(Name = "User Role")]
        [Required(ErrorMessage = "Role Required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Role must be between 3 and 50 characters!")]
        public string Role { get; set; }

        [Display(Name = "Role Description")]
        [Required(ErrorMessage = "Role Description Required")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Role must be between 3 and 255 characters!")]

        public string RoleDescription { get; set; }

    }
}