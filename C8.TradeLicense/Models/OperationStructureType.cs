using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class OperationStructureType:BaseModel
    {
        [Key]
        public int OperationStructureTypeId { get; set; }
        [Display( Name = "Operation Structure" )]
        [Required(ErrorMessage = "Operation Structure Required")]
        public string OperationStructureTypeName { get; set; }
        [Display( Name = "Operation Structure Description" )]
        public string OperationStructureTypeDescription { get; set; }
    }
}