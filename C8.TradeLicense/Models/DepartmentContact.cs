using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using C8.TradeLicense.DataAccessLayer;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class DepartmentContact : BaseModel
    {
        [Key]
        public int DepartmentContactId { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department _DepartmentId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public bool IsPrinciple { get; set; }

        public int? DepartmentHeadContactId { get; set; }
        [ForeignKey("DepartmentHeadContactId")]
        public DepartmentContact DepartmentHeadContact { get; set; }

        [StringLength(100)]
        public string RoleName { get; set; }
    }
}