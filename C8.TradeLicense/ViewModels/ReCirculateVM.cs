using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.ViewModels
{
    public class ReCirculateVM
    {
        public int InspectionRequestId { get; set; }

        public int? CurrentInspectorId { get; set; }

        public int NewInspectorId { get; set; }

        public string Reason { get; set; }

        public List<SelectListItem> Inspectors { get; set; }
    }
}