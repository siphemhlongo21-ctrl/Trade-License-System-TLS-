using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class ItemCondition: BaseModel
    {

        [Key]
        public int ItemConditionId { get; set; }

        public int ItemTypeId { get; set; }
        [ForeignKey("ItemTypeId")]
        public ItemType ItemType { get; set; }

        [Display(Name="Condition Name")]
        [Required]
        public string ItemConditionName { get; set; }
        [Display(Name="Condition Description")]
        public string ItemConditionDescription { get; set; }

    }
}