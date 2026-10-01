using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class Condition:BaseModel
    {

        [Key]
        public int ConditionId { get; set; }

        //[Required]
        //public int ItemConditionId { get; set; }
        //[ForeignKey("ItemConditionId")]
        //public ItemCondition ItemCondition { get; set; }

        [Display(Name="Condition Name")]
        [Required]
        public string ConditionName { get; set; }

        [Display(Name="Condition Description")]
        public string ConditionDescription { get; set; }
    }
}