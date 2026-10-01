using System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using C8.TradeLicense.Models;
namespace C8.TradeLicense.Models
{
    public class ItemSubCategory : BaseModel
    {

        [Key]
        public int ItemSubCategoryId { get; set; }

        [Required(ErrorMessage = "Please select Item Type.")]
        public int? ItemTypeId { get; set; }
        [ForeignKey("ItemTypeId")]
        public ItemType ItemType { get; set; }

        [Required(ErrorMessage = "Please Enter Sub-category name.")]
        [Display(Name = "Item Sub-Category Name")]
        public string ItemSubCategoryName { get; set; }
        [Display(Name = "Item Sub-Category Description")]
      
        public string ItemSubCategoryDescription { get; set; }
    }
}