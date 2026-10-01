using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class PaymentLicense:BaseModel
    {

        [Key]
        public int Id { get; set; }

        public int LicenseId { get; set; }
        [ForeignKey("LicenseId")]
        public License License { get; set; }


        [StringLength(50)]
        [Display(Name = "Customer Account Number")]
        public string CUST_ACCT_NO { get; set; }

        [StringLength(50)]
        [Display(Name = "Service Unit")]
        public string SERVICE_UNIT { get; set; }

        [StringLength(50)] //Reference Number column will be nvarchar(50)
        [Display(Name = "Request Number")]
        public string REQUEST_NO { get; set; }

        [StringLength(50)]
        [Display(Name = "Pay in slip Number")]
        public string PAYINSLIP_NO { get; set; }

        
        [Display(Name = "Pay in slip Amount")]
        public Double PAYINSLIP_AMOUNT { get; set; }



      
        [Display(Name = "Allocated Amount")]
        public Double ALLOCATED_AMOUNT { get; set; }

        [Display(Name = "Paid Amount")]
        public Double PAID_AMOUNT { get; set; }


        [Display(Name = "Unallocated Amount")]
        public Double BALANCE_UNALLOCATED_AMOUNT { get; set; }


        [StringLength(10)]
        [Display(Name = "Pay In Slip Date")]
        public string PAYINSLIP_DATE { get; set; }

        [StringLength(10)]
        [Display(Name = "Paid Date")]
        public string PAID_DATE { get; set; }
    }
    }