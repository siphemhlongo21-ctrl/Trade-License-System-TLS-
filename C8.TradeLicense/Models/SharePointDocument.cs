using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class SharePointDocument
    {
        public string DocumentId { get; set; }
        public string DocumentUrl { get; set; }
        public byte[] FileContent { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}