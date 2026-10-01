using C8.TradeLicense.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.ViewModels
{
    public class DepartmentCirculationHistoryVM
    {
        public IEnumerable<InspectionRequest> InspectionRequests { get; set; }
        public IEnumerable<DepartmentContact> DepartmentContacts { get; set; }
        public IEnumerable<FileUpload> InspectionRequestUploadList { get; set; }
        
    }
}