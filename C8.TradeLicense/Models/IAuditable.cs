using System;
using System.ComponentModel.DataAnnotations;

namespace C8.TradeLicense.Models
{
    public interface IAuditable
    {
        
        int? CreatedByUserId { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        DateTime? CreatedDateTime { get; set; }
        int? ModifiedByUserId { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        DateTime? ModifiedDateTime { get; set; }
    }
}