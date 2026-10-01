using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class ServiceLevelAgreement: BaseModel
    {
        [Key]
        public int SlaId { get; set; }
        public string SlaKey { get; set; }
        public string SlaName { get; set; }
        public string SlaDescription { get; set; }
    
    }
}