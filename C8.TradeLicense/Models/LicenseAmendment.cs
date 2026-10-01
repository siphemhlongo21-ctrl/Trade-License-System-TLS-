using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Models
{
    public class LicenseAmendment
    {
        [Key]
        public int AmendmentId { get; set; }

        public int LicenseId { get; set; }

        public License License { get; set; }
        public int ClientId { get;set; }
        public Client Client { get; set; }
        public int BusinessId { get; set; }
        public Business Business { get; set; }

        public string AmendmentType { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }

        public string AmendmentDataJson { get; set; }

        public int RequestedByUserId { get; set; }

        public DateTime RequestedDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public DateTime? RejectedDate { get; set; }

        public string RejectionReason { get; set; }
    }
}