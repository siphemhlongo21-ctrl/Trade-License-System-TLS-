using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using C8.TradeLicense.Models;


namespace C8.TradeLicense.Models
{
    public class FileUpload : BaseModel
    {
        [Key]
        public int FileUploadId { get; set; }

        public int? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "Uploadedby")]
        public string Uploadedby { get; set; }

        [Display(Name = "Upload date")]
        public string Uploadeddate { get; set; }

        [Display(Name = "File Path")]
        public string FilePath { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Display(Name = "FileType")]
        public int DocumentId { get; set; }
        [ForeignKey("DocumentId")]
        public Document Document { get; set; }

        public int referenceId { get; set; }
    }
}