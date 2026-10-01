using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using C8.TradeLicense.Models;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace C8.TradeLicense.ViewModels
{
    public class LicenseDocumentViewModels
    {
        public IEnumerable<LicenseType> LicenseTypes { get; set; }
        public IEnumerable<DocumentType> DocumentTypes { get; set; }
    }
}