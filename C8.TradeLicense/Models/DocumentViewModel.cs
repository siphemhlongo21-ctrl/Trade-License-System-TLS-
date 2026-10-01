using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class DocumentViewModel
    {
        public byte[] FileContent { get; set; }
        public string SharePointSite { get; set; }
        public string SharePointLibrary { get; set; }
        public string FileName { get; set; }
        public string Application { get; set; }
        public string ReferenceId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}