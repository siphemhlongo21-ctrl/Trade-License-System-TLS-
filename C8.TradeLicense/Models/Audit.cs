using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class Audit
    {
        [Key]
        public int AuditId { get; set; }


        public string Action { get; set; }
        public int PrimaryKey { get; set; }
        public int LicenseId { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string OriginalValue { get; set; }
        public string CurrentValue { get; set; }
        public int? AuditByUserId { get; set; }
        public DateTime AuditDateTime { get; set; }
    }

    public class AuditEnitity
    {
        public Audit Audit { get; set; }
        public object Entity { get; set; }
    }
}