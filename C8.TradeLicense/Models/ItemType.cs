using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using C8.TradeLicense.Models;

namespace C8.TradeLicense.Models
{
    public class ItemType : BaseModel
    {
        [Display(Name = "Item Type")]
        [Key]
        public int ItemTypeId { get; set; }
        [Display(Name = "Item Type Name")]

        [Required(ErrorMessage = "Please enter Item Type name.")]
        public string ItemTypeName { get; set; }

        [Display(Name = "Item Type Description")]
        public string ItemTypeDescription { get; set; }

        [Required(ErrorMessage = "Please enter Item Type Key.")]
        [Display(Name="Item Type Key")]
        public string ItemTypeKey { get; set; }
    }
}