using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.ViewModels
{
    public class UploadRequestModel
    {
        public UploadRequest UploadRequest { get; set; }
    }

    public class UploadRequest
    {
        public string FileName { get; set; }
        public string DestinationFolderName { get; set; }
        public string DocumentType { get; set; }
        public string DocumentSubType { get; set; }
        public string ReferenceNumber { get; set; }
        public string Description { get; set; }
        public byte[] FileContent { get; set; }
    }
}