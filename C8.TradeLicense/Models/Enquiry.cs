using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using C8.TradeLicense.DataAccessLayer;
using System.Linq;
using System.Web;
namespace C8.TradeLicense.Models
{
    public class Enquiry : BaseModel
    {
        [Key]
        public int EnquiryId { get; set; }

        [Display(Name = "Client Name")]
        [Required]
        public string ClientName { get; set; }

        [Display(Name = "Trading Name")]
        [Required]
        public string ProposedTradeName { get; set; }
        [Display(Name = "Postal Address")]
        public string PostalAddress1 { get; set; }
        [Display(Name = "Suburb")]
        public string PostalAddress2 { get; set; }
        [Display(Name = "City")]
        public string PostalAddress3 { get; set; }
        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        public int PostalAddressCode { get; set; }

        [Display(Name = "Postal Address")]
        public string PostalAddress
        {
            get
            {
                return string.Format("{0}, {1}, {2}, {3}", PostalAddress1, PostalAddress2, PostalAddress3,
                                     PostalAddressCode);
            }
        }

        [Display(Name = "Department")]
        //[Required(ErrorMessage = "Please select License Type.")]
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }

        [Display(Name = "Type of License")]
        [Required(ErrorMessage = "Please select License Type.")]
        public int LicenseTypeId { get; set; }
        [ForeignKey("LicenseTypeId")]
        public LicenseType LicenseType { get; set; }

        [Display(Name = "Enquiry")]
        [Required]
        public string EnquiryReport { get; set; }

        [Display(Name = "Inspectors Report")]
        [Required]
        public string InspectorsReport { get; set; }
    }
}