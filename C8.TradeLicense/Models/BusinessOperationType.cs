using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class BusinessOperationType:BaseModel
    {
        [Key]
        public int BusinessOperationTypeId { get; set; }
        [Display( Name = "Business Operation" )]
        public string BusinessOperationTypeName { get; set; }
        [Display( Name = "Business Operation Description" )]
        public string BusinessOperationTypeDescription { get; set; }
    }
}