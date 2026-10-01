using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using C8.TradeLicense.Models;

namespace C8.TradeLicense.ViewModels
{
    public class RegisterClientBusinessViewModel
    {
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<Business> Businesses { get; set; }
 
    }
}