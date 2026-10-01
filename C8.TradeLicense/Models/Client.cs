using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.Models
{
    public class Client : BaseModel
    {
        [Key]
        public int ClientId { get; set; }

        [Display(Name = "ID/ Passport No.")]
        [StringLength(50)]
        [Remote("IsClientIdNumberExist", "Client", HttpMethod = "POST", ErrorMessage = "ID Number/Passport already exists in the system!")]

      
        public string IdentityOrPassportNumber { get; set; }

        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Client Name field should consist of characters only")]
        [Display(Name = "Customer Name")]
        [StringLength(80)]
        public string Name { get; set; }

        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Client Surname field should consist of characters only")]
        [Display(Name = "Customer Surname")]
        [StringLength(80)]
        public string Surname { get; set; }

        [Display(Name = " Individual Type")]
        [StringLength(50)]
        public string Individual { get; set; }

        [Display(Name = " Customer Type")]
        [StringLength(50)]
        public string CustomerType{ get; set; }

        [Display(Name = " Business Reg Number")]
        [StringLength(50)]
        public string RegNumber{ get; set; }

        [Display(Name = " Business Name")]
        [StringLength(200)]
        public string BusinessName { get; set; }

        public bool SameAs { get; set; }

        [Display(Name = "Nationality")]
        [StringLength(50)]
        public string Nationality { get; set; }

        [Display(Name = "Other")]
        [StringLength(50)]
        public string NationalityOther { get; set; }

        [Display(Name = "Expiry of permit")]
        [StringLength(50)]
        public string ExpiryPermit { get; set; }

        [Display(Name = "Customer")]
        [StringLength(160)]
        public string Fullname { get { return string.Format("{0} {1}", Name, Surname); } }

        [Display(Name = "Residential Address"), Required]
        [StringLength(100)]
        public string ResidentialAddress1 { get; set; }

        [Display(Name = "Suburb"), Required]
        [StringLength(100)]
        public string ResidentialAddress2 { get; set; }
        [StringLength(100)]

        [Display(Name = "City"), Required]
        
        public string ResidentialAddress3 { get; set; }
    

        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "Please enter a valid code.")]
        [Display(Name = "Code"), Required]
        public int ResidentialAddressCode { get; set; }

        [Display(Name = "Residential Address"), Required]
        [StringLength(500)]
        public string ResidentialAddress
        {
            get
            {
                return string.Format("{0}, {1}, {2}, {3}", ResidentialAddress1, ResidentialAddress2, ResidentialAddress3,
                                     ResidentialAddressCode);
            }
        }
        [Display(Name = "Postal Address"), Required]
        [StringLength(100)]
        public string PostalAddress1 { get; set; }

        [Display(Name = "Suburb"), Required]
        [StringLength(100)]
        public string PostalAddress2 { get; set; }

        [Display(Name = "City"), Required]
        [StringLength(100)]
        public string PostalAddress3 { get; set; }

        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "Please enter a valid code.")]
        [Display(Name = "Code"), Required]
        public int PostalAddressCode { get; set; }

        [Display(Name = "Postal Address")]
        [StringLength(500)]
        public string PostalAddress
        {
            get
            {
                return string.Format("{0}, {1}, {2}, {3}", PostalAddress1, PostalAddress2, PostalAddress3,
                                     PostalAddressCode);
            }
        }

        
        [DataType(DataType.PhoneNumber), Display(Name = "Customer  Telephone")]
        [StringLength(100)]
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "Please enter a valid phone number.")]
        public string TelephoneNumber { get; set; }

    
        [MaxLength(10, ErrorMessage = "Please enter a valid cellphone number.")]
        [StringLength(100)]
        [Display(Name = "Customer Cellphone No.")]
        [RegularExpression(@"[0-9]*\.?[0-9]", ErrorMessage = "Please enter a valid cellphone number.")]
        //[DataType(DataType.PhoneNumber)]
        public string CellphoneNumber { get; set; }

        [MaxLength(10, ErrorMessage = "Please enter a valid cellphone number.")]
        [StringLength(100)]
        [Display(Name = "Customer Alternative Cellphone No.")]
        [RegularExpression(@"[0-9]*\.?[0-9]", ErrorMessage = "Please enter a valid cellphone number.")]
 
        public string AltCellphoneNumber { get; set; }

        [Display(Name = "Customer Fax No.")]
        [StringLength(100)]
        
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "Please enter a valid fax number.")]
        public string FaxNumber { get; set; }
        
      
        [Display(Name = "Customer Email Address")]
        [StringLength(100)]
        [Required(ErrorMessage = "Email Address is required")]       
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }



        public int ClientStatusId { get; set; }
        [ForeignKey("ClientStatusId")]

        public Status ClientStatus { get; set; }
    }
}