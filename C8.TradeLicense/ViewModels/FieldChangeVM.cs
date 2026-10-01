using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.ViewModels
{
    public class FieldChangeVM
    {
        public string TableName { get; set; }

        public string FieldName { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }
    }
}