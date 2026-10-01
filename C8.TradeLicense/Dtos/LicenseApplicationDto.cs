using C8.TradeLicense.Models;
using C8.TradeLicense.Models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Dtos
{
    public class LicenseApplicationDto
    {
        public AmendmentType AmendmentType { get; set; }
        public int LicenseId { get; set; }
        public virtual License License { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
        public int BusinessId { get; set; }
        public virtual Business Business { get; set; }
        public string LicenseType { get; set; }
        public int? BusinessOperatorId { get; set; }
        public BusinessManger Operator { get; set; }
        public int RequestedByUserId { get; set; }
    }
}