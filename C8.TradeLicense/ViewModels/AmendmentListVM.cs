using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace C8.TradeLicense.ViewModels
{
    public class AmendmentListVM
    {
        public int AmendmentId { get; set; }

        public int LicenseId { get; set; }
        public string LicenseNumber { get; set; }   

        public string AmendmentType { get; set; }

        public DateTime RequestedDate { get; set; }

        public string Status { get; set; }

        public string BusinessName { get; set; }
        public IEnumerable<C8.TradeLicense.Models.FileUpload> BusinessDocuments { get; set; }
    }
}