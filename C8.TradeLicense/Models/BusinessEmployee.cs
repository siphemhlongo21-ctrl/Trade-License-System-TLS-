using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class BusinessEmployee : BaseModel
    {
        [Key]
        public int BusinessEmployeeId { get; set; }

        public int BusinessId { get; set; }
        [ForeignKey("BusinessId")]
        public Business Business { get; set; }

      
       


        [Display(Name = "Employer Name")]
        [StringLength(100)]
        public string NameOfEmployer { get; set; }

        [Display(Name = "Employer Address")]
        [StringLength(100)]
        public string EmployerResidentialAddress1 { get; set; }

        [Display(Name = "Suburb")]
        [StringLength(100)]
        public string EmployerResidentialAddress2 { get; set; }
        [Display(Name = "City")]
        [StringLength(100)]
        public string EmployerResidentialAddress3 { get; set; }
        [Display(Name = "Code")]
        [DataType(DataType.PostalCode)]
        public int? EmployerResidentialAddressCode { get; set; }
   
       

     

      

    }
}