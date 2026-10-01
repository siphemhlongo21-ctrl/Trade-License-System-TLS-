using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.ViewModels
{
    public class AmendmentReviewVM
    {
        public int AmendmentId { get; set; }

        public int LicenseId { get; set; }
        public string LicenseNumber { get; set; }

        public string AmendmentType { get; set; }

        public string BusinessName { get; set; }

        public List<FieldChangeVM> Changes { get; set; }
        public string ApprovalDisplay { get;set;  }
        public string RejectionDisplay { get; set; }
        public List<Models.FileUpload> BusinessDocuments { get; set; }
    }
}