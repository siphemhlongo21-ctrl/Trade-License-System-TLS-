using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class ItemCategory: BaseModel
    {
        [Key]
        public int ItemCategoryId { get; set; }

        [Display(Name = "Item Type")]
        public int? ItemTypeId { get; set; }
        [ForeignKey("ItemTypeId")]
        public ItemType ItemType { get; set; }

        [Display(Name = "Item Category Name")]
        public string ItemCategoryName { get; set; }
        [Display(Name = "Item Category Description")]
        public string ItemCategoryDescription { get; set; }
    }
}