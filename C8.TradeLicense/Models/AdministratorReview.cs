using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.Models
{
    public class AdministratorReview : BaseModel
    {
        [Key]
        public int AdministratorReviewId { get; set; }

        public int LicenseId { get; set; }
        [ForeignKey("LicenseId")]
        public License License { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Display(Name = "Decision")]
        [StringLength(50)]
        public string Decision { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Date Reviewed")]
        public DateTime DateReviewed { get; set; }
    }
}