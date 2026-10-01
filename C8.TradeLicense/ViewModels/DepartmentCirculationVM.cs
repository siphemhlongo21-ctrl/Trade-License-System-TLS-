using C8.TradeLicense.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.ViewModels
{
    public class DepartmentCirculationVM
    {
        public License LicenseDetails { get; set; }
        public Business BusinessDetails { get; set; }
        public Client CustomerDetails { get; set; }
        public IEnumerable<FileUpload> ClientUploadList { get; set; }
       // public SelectList DepartmentList { get; set; }
        public IEnumerable<Document> ClientOustandingDocuments { get; set; }
        public IEnumerable<PaymentLicense> PaymentLicenses { get; set; }
        public DepartmentCirculationHistoryVM DepartmentCirculationHistoryVM { get; set; }
    }
}