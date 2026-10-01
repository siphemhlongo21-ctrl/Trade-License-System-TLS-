using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class TitleDeedType : BaseModel
    {
        [Key]
        public int TitleDeedTypeId { get; set; }
        [Display( Name = "Title Deed" )]
        [Required(ErrorMessage = "Please enter Title Deed Name.")]
        public string TitleDeedTypeName { get; set; }

        [Display( Name = "Title Deed Description" )]

        [DataType(DataType.MultilineText)]
        public string TitleDeedDescription { get; set; }
    }
}