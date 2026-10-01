using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class LicenseDocuments
    {
        public IEnumerable<LicenseType> LicenseTypeDetails { get; set; }
        public IEnumerable<DocumentType> DocumentTypeDetails { get; set; }
    }
}