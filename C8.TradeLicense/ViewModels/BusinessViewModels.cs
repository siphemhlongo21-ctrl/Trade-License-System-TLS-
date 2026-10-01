using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using C8.TradeLicense.Models;

namespace C8.TradeLicense.ViewModels
{
    public class BusinessViewModels
    {
        public Business BusinessDetails { get; set; }

        public BusinessEmployee BusinessEmployeeDetails { get; set; }

        public BusinessManger BusinessMangerDetails { get; set; }

    
    }
}