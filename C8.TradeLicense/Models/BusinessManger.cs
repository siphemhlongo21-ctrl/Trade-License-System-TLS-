using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class BusinessManger : BaseModel
    {
        [Key]
        public int BusinessMangerId { get; set; }

        public int BusinessId { get; set; }
        [ForeignKey("BusinessId")]
        public Business Business { get; set; }

      
        [Display(Name = "Business Operator")]
        [StringLength(200)]
        public string NameOfBusinessOperator { get; set; }
        [Display(Name = "Operator ID No/ Passport")]
        [StringLength(50)]
        public string OperatorIdentityOrPassportNumber { get; set; }

     

        [Display(Name = "Operator Address")]
        [StringLength(100)]
        public string OperatorResidentialAddress1 { get; set; }

       
        [Display(Name = "Suburb")]
        [StringLength(100)]
        public string OperatorResidentialAddress2 { get; set; }
        [Display(Name = "City")]
        [StringLength(100)]
        public string OperatorResidentialAddress3 { get; set; }

        [Display(Name = "Code")]
        [DataType(DataType.PostalCode)]
        public int? OperatorResidentialAddressCode { get; set; }

        [Display(Name = "Operator Residential Address")]
        [StringLength(300)]
        public string OperatorResidentialAddress
        {
            get
            {
                return string.Format("{0}, {1}, {2}, {3}", OperatorResidentialAddress1, OperatorResidentialAddress2, OperatorResidentialAddress3,
                                     OperatorResidentialAddressCode);
            }
        }
       


   
       

     

        [Display(Name = "Business Operator Type")]
        public int? BusinessOperatorId { get; set; }
        [ForeignKey("BusinessOperatorId")]
        public BusinessOperator BusinessOperator { get; set; }

    }
}