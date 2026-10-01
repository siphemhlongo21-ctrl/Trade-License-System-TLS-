using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class BusinessOperator:BaseModel
    {
        [Key]
        public int BusinessOperatorId { get; set; }

        [DisplayName("Business Operator Name")]
        public string BusinessOperatorName { get; set; }
        [DisplayName("Business Operator Description")]
        [DataType(DataType.MultilineText)]
        public string BusinessOperatorDescription { get; set; }

        
    }
}