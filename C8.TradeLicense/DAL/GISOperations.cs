using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using C8.TradeLicense.GIS;
namespace C8.TradeLicense.DAL
{
    public class GISOperations
    {
        public GISResponse_T SearchAddressRequest(string streetname)
        {
            GIS.GISResponse_T res = new GISResponse_T();
            try
            {
                GIS.PortTypeClient serviceGetCustomer = new GIS.PortTypeClient();

                GIS.GIS_Request_T gis = new GIS.GIS_Request_T();
                if(streetname != null)
                gis.Erf = streetname.Trim() == string.Empty ? "%" : streetname.Trim();
                

                res = serviceGetCustomer.Operation(gis);                
                //if (res.Status.StatusMSG == "Success")
                //{
                //    return (res);
                //};
                return res;
                
            }
            catch(Exception e)
            {
                Status_T exceptionStatus = new Status_T();
                exceptionStatus.StatusCode = "404";
                exceptionStatus.StatusMSG = "Service down error, try again and if error persist contact administrator";
                res.Status = exceptionStatus;
                var em = e.Message;
                return res;
            }
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "openGISModal", "openGISModal();", true);

        }
    }
}