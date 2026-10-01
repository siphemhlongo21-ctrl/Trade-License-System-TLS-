using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.Models
{
    public class MigratedClient : BaseModel
    {
        [Key]
        public int MClientId { get; set; }

        [Display(Name = "ID/ Passport No.")]
        [StringLength(50)]
       

      
        public string IdentityOrPassportNumber { get; set; }

       
        [Display(Name = "Customer Name")]
        [StringLength(80)]
        public string Name { get; set; }

       
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

        public bool? SameAs { get; set; }

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

        [Display(Name = "Residential Address")]
        [StringLength(100)]
        public string ResidentialAddress1 { get; set; }

        [Display(Name = "Suburb")]
        [StringLength(100)]
        public string ResidentialAddress2 { get; set; }
        [StringLength(100)]

        [Display(Name = "City")]
        
        public string ResidentialAddress3 { get; set; }
    

      
        [Display(Name = "Code")]
        public int? ResidentialAddressCode { get; set; }

        [Display(Name = "Residential Address")]
        [StringLength(500)]
        public string ResidentialAddress
        {
            get
            {
                return string.Format("{0}, {1}, {2}, {3}", ResidentialAddress1, ResidentialAddress2, ResidentialAddress3,
                                     ResidentialAddressCode);
            }
        }
        [Display(Name = "Postal Address")]
        [StringLength(100)]
        public string PostalAddress1 { get; set; }

        [Display(Name = "Suburb")]
        [StringLength(100)]
        public string PostalAddress2 { get; set; }

        [Display(Name = "City")]
        [StringLength(100)]
        public string PostalAddress3 { get; set; }

   
        [Display(Name = "Code")]
        public int? PostalAddressCode { get; set; }

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
        
        public string TelephoneNumber { get; set; }

    
    
        [StringLength(100)]
        [Display(Name = "Customer Cellphone No.")]
       
        public string CellphoneNumber { get; set; }

        [StringLength(100)]
        [Display(Name = "Customer Alternative Cellphone No.")]

        public string AltCellphoneNumber { get; set; }

        [Display(Name = "Customer Fax No.")]
        [StringLength(100)]
        
       
        public string FaxNumber { get; set; }
        
     
        [Display(Name = "Customer Email Address")]
        [StringLength(100)]
        public string EmailAddress { get; set; }



        public int? ClientStatusId { get; set; }
        [ForeignKey("ClientStatusId")]

        public Status ClientStatus { get; set; }
    }
}