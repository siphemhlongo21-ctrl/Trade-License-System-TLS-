using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.Models
{
    public class InspectionAppeal
    {
        [Key]
        public int InspectionAppealId { get; set; }

        [Required]
        [ForeignKey("InspectionResponse")]
        [Display(Name = "InspectionResponseId")]
        public int InspectionResponseId { get; set; }
        public virtual InspectionResponse InspectionResponse{ get; set; }
        public bool SameAs { get; set; }


        [Required]
        [ForeignKey("License")]
        [Display(Name = "License")]
        public int LicenseId { get; set; }

        public virtual License License { get; set; }

        [Display(Name = "ID/ Passport No."), Required]
        [Remote("IsClientIdNumberExist", "Client", HttpMethod = "POST", ErrorMessage = "ID Number/Passport already exists in the system!")]


        public string IdentityOrPassportNumber { get; set; }

        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Client Name field should consist of characters only")]
        [Display(Name = "Appeallent Name"), Required]
        public string Name { get; set; }

        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Client Surname field should consist of characters only")]
        [Display(Name = "Appeallent Surname"), Required]
        public string Surname { get; set; }

        [Display(Name = " Individual Type")]
        public string Individual { get; set; }

        [Display(Name = "Nationality")]
        public string Nationality { get; set; }

        [Display(Name = "Other")]
        public string NationalityOther { get; set; }

        [Display(Name = "Expiry of permit")]
        public string ExpiryPermit { get; set; }

        [Display(Name = "Appeallent")]
        public string Fullname { get { return string.Format("{0} {1}", Name, Surname); } }

        [Display(Name = "Residential Address"), Required]
        public string ResidentialAddress1 { get; set; }

        [Display(Name = "Suburb"), Required]
        public string ResidentialAddress2 { get; set; }

        [Display(Name = "City"), Required]
        public string ResidentialAddress3 { get; set; }

        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "Please enter a valid code.")]
        [Display(Name = "Code"), Required]
        public int ResidentialAddressCode { get; set; }

        [Display(Name = "Residential Address"), Required]
        public string ResidentialAddress
        {
            get
            {
                return string.Format("{0}, {1}, {2}, {3}", ResidentialAddress1, ResidentialAddress2, ResidentialAddress3,
                                     ResidentialAddressCode);
            }
        }
        [Display(Name = "Postal Address"), Required]
        public string PostalAddress1 { get; set; }

        [Display(Name = "Suburb"), Required]
        public string PostalAddress2 { get; set; }

        [Display(Name = "City"), Required]
        public string PostalAddress3 { get; set; }

        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "Please enter a valid code.")]
        [Display(Name = "Code"), Required]
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

        [MinLength(7, ErrorMessage = "Please enter a valid phone number.")]
        [DataType(DataType.PhoneNumber), Display(Name = "Appeallent  Telephone")]
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "Please enter a valid phone number.")]
        public string TelephoneNumber { get; set; }

        [Required]
        [MinLength(10, ErrorMessage = "Please enter a valid cellphone number.")]
        [Display(Name = "Appeallent Cellphone No.")]
        [RegularExpression(@"[0-9]*\.?[0-9]", ErrorMessage = "Please enter a valid cellphone number.")]
        //[DataType(DataType.PhoneNumber)]
        public string CellphoneNumber { get; set; }

        [Display(Name = "Appeallent Fax No.")]
        [MinLength(7, ErrorMessage = "Please enter a valid fax number.")]
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "Please enter a valid fax number.")]
        public string FaxNumber { get; set; }

        [Required]
        [Display(Name = "Appeallent Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }

       

        [Display(Name = "Appeal Reason")]
        public string Reason { get; set; }

        [Display(Name = "Appeal Captured Date")]
        public DateTime? AppealDate { get; set; }

        [Display(Name = " Appeal doc Date")]
        public DateTime? AppealDocDate { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime? AppealDateTime { get; set; }
        [Display(Name = "Appeal status")]
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public Status Status { get; set; }

    }
}
