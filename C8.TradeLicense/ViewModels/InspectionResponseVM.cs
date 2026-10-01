using C8.TradeLicense.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.ViewModels
{
    public class InspectionResponseVM
    {
        public IEnumerable<InspectionResponse> InspectionResponses { get; set; }
        public IEnumerable<DepartmentContact> DepartmentContacts { get; set; }
        public IEnumerable<FileUpload> InspectionResponseDocuments { get; set; }


    }
}