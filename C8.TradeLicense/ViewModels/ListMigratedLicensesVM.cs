using C8.TradeLicense.Models;
using C8.TradeLicense.Models.Interfaces;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.ViewModels
{
    public class ListMigratedLicensesVM:IUIMessage
    {
        public IPagedList<MigratedLicense> MigratedLicenses { get; set; }
        public string InputSearch { get; set; }
        public string SelectedSearch { get; set; }
        public int? Page { get; set; }
        public int LicenseCount { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; }
    }
}