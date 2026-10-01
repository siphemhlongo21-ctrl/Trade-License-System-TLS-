using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class MigratedBusiness : BaseModel
    {
        [Key]
        public int MBusinessId { get; set; }

        public int? MClientId { get; set; }
        [ForeignKey("MClientId")]
        public MigratedClient MigratedClient { get; set; }

        [Display(Name = "Trading Name")]

        [StringLength(200)]
        public string ProposedTradeName { get; set; }

        [Display(Name = "Known As")]
        [StringLength(200)]
        public string KnownAs { get; set; }

        public bool? SameAs { get; set; }

        [Display(Name = "Business Type")]
        public int? BusinessTypeId { get; set; }
        [ForeignKey("BusinessTypeId")]
        public BusinessType BusinessType { get; set; }

 
        [Display(Name = "Postal Address")]
        [StringLength(100)]
        public string PostalAddress1 { get; set; }

     
        [Display(Name = "Suburb")]
        [StringLength(100)]
        public string PostalAddress2 { get; set; }
        [Display(Name = "City")]
        [StringLength(100)]
        public string PostalAddress3 { get; set; }
        [Display(Name = "Postal Code")]
      
        [DataType(DataType.PostalCode)]
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

        [Display(Name = "Shop / Unit Number")]
        public int? ResidentialShopOrUnitNumber { get; set; }
        
        [Display(Name = "Business Address ")]
        [StringLength(100)]
        public string ResidentialStreetAddress { get; set; }

        [Display(Name = "Business Address")]
        [StringLength(100)]
        public string ResidentialAddress1 { get; set; }

     
        [Display(Name = "Suburb")]
        [StringLength(100)]
        public string ResidentialAddress2 { get; set; }
        [Display(Name = "City")]
        [StringLength(100)]
        public string ResidentialAddress3 { get; set; }
        [Display(Name = "Code")]
        [DataType(DataType.PostalCode)]
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

        
        [Display(Name = "Telephone Number")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(50)]
        public string TelephoneNumber { get; set; }

        [Display(Name = "Cellphone Number")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(50)]
        public string CellphoneNumber { get; set; }

        [Display(Name = "Alternative Cellphone Number")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(50)]
        public string AltCellphoneNumber { get; set; }

        //[MinLength(7, ErrorMessage = "Please enter a valid fax number.")]

        [Display(Name = "Fax Number")]
        [StringLength(50)]
        public string FaxNumber { get; set; }

        [Display(Name = "Title Deed Type")]
        public int? TitleDeedTypeId { get; set; }
        [ForeignKey("TitleDeedTypeId")]

        public TitleDeedType TitleDeedType { get; set; }

        [Display(Name = "Rates Account Number")]
        [StringLength(100)]
        public string RatesAccountNumber { get; set; }

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
        [Display(Name = "Item Type")]
        public int? ItemTypeId { get; set; }
        [ForeignKey("ItemTypeId")]
        public ItemType ItemType { get; set; }

        [Display(Name = "Employer")]
        public bool? IsSelfEmployed { get; set; }

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
        [Display(Name = "Operation Structure Type")]
        public int? OperationStructureTypeId { get; set; }
        [ForeignKey("OperationStructureTypeId")]
        public OperationStructureType OperationStructureType { get; set; }

        public int? BusinessStatusId { get; set; }
        [ForeignKey("BusinessStatusId")]
        public Status BusinessStatus { get; set; }

        [Display(Name = "Business Operator Type")]
        public int? BusinessOperatorId { get; set; }
        [ForeignKey("BusinessOperatorId")]
        public BusinessOperator BusinessOperator { get; set; }

    }
}