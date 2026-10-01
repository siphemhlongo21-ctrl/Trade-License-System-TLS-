using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class EscalationLog
    {
        [Key]
        public int ID { get; set; }

        public int EventID { get; set; }

        public DateTime? EventDateTime { get; set; }
    }
}