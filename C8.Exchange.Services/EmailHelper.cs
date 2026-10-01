using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace C8.Exchange.Services
{
    public class EmailHelper
    {

        public string ConstructEmailBody(string userName, string msg)
        {
            string body = string.Empty;

            //var reader1 = File.ReadAllText("~/_EmailTemplatePartial.cshtml");
            var reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/_EmailTemplatePartial.cshtml"));

            body = reader.ReadToEnd();
            body = body.Replace("{UserName}", userName);
            body = body.Replace("{Body}", msg);
            return body;
        }

        public string EmailTemplateAsString()
        {
            var template =
                new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/_EmailTemplatePartial.cshtml"));
            var stringTemplate = template.ReadToEnd();
            return stringTemplate;
        }
    }
}
