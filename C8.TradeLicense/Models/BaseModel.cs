using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class BaseModel : IAuditable
    {
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }

        public int? CreatedByUserId { get; set; }
        [ForeignKey( "CreatedByUserId" )]
        [Display( Name = "Created By" )]
        public User CreatedByUser { get; set; }
        [DisplayFormat( DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true )]
        [Display( Name = "Created On" )]
        public DateTime? CreatedDateTime { get; set; }

        public int? ModifiedByUserId { get; set; }
        [ForeignKey( "ModifiedByUserId" )]
        [Display( Name = "Modified By" )]
        public User ModifiedByUser { get; set; }
        [DisplayFormat( DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true )]
        [Display( Name = "Modified On" )]
        public DateTime? ModifiedDateTime { get; set; }
    }
}