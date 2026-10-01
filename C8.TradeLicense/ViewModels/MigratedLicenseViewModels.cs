using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using C8.TradeLicense.Models;

namespace C8.TradeLicense.ViewModels
{
    public class MigratedLicenseViewModels
    {
        public MigratedLicense LicenseDetails { get; set; }

        public MigratedBusiness BusinessDetails { get; set; }

        public MigratedClient CustomerDetails { get; set; }

    
    }
}