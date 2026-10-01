using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class User : BaseModel
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please enter Name.")]
        //[RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "The FirstName field should consist of characters only")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter Last Name.")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "The Last Name field should consist of characters only")]
        public string LastName { get; set; }

        //[RegularExpression(@"^[a-zA-Z. ]+$", ErrorMessage = "The Username field should consist of characters only")]

        [Required(ErrorMessage = "Please enter Username.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter valid Email Address.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }

        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "The LandLine field should consist of Numbers only")]
        public string LandLine { get; set; }

        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "The Mobile field should consist of Numbers only")]
        public string Mobile { get; set; }

        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "The Fax field should consist of Numbers only")]
        public string Fax { get; set; }

        public string IpAddress { get; set; }

        [Display(Name = "Region")]
        [Required(ErrorMessage = "Please Select a Region.")]
        public int Region { get; set; }
        [ForeignKey("Region")]
        public Region RegionId { get; set; }


        [Display(Name = "Role")]
        [StringLength(128)]
        public string Role { get; set; }
      
      


        public bool? IsPasswordReset { get; set; }

        [Display(Name = "Fullname")]
        public string FullName
        {
            get { return string.Format( "{0} {1}", FirstName, LastName ); }
        }
    }
}