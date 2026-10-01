using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using C8.TradeLicense.Models;
using System.Data.Entity.Core.Objects;
using C8.TradeLicense.DataAccessLayer;


namespace C8.TradeLicense.DAL
{
    public class RegionPrefix
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        public string GetRegionPrefix(int processID, string region)
        {
            string regionPrefix = "";
            switch (region)
            {
                case "Central":
                    regionPrefix = "BO";
                    break;
                case "Central North":
                    regionPrefix = "CN";
                    break;
                case "Central South":
                    regionPrefix = "CS";
                    break;
                case "North":
                    regionPrefix = "BN";
                    break;
                case "South":
                    regionPrefix = "BLS";
                    break;
                case "West":
                    regionPrefix = "BW";
                    break;
                case "Mpumalanga":
                    regionPrefix = "BLW";
                    break;
            }
            var refNo = new object();
            try
            {

               
        
        ObjectParameter case_desc = new ObjectParameter("case_desc", "");

        //getRef.cmn_casecounter_sp(processID, regionPrefix, case_desc);
        var Licensescount = db.Licenses.Count();
                Licensescount = ++Licensescount;

                var currentdate = DateTime.Now.ToString("yyMMdd");



                refNo = regionPrefix + currentdate + Licensescount;
                //refNo = case_desc.Value;

            }
            catch (Exception ee)
            {
                var em = ee.Message;
            }
            return refNo.ToString();
        }
    }
}