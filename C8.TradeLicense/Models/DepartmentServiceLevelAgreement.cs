using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class DepartmentServiceLevelAgreement: BaseModel
    {
        [Key]
        public int DepartmentSlaId { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }

        public int SlaId { get; set; }
        [ForeignKey("SlaId")]
        public ServiceLevelAgreement ServiceLevelAgreement { get; set; }

        public int SlaDays { get; set; }
    }
}