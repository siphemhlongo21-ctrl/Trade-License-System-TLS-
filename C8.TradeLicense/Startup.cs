using System.Web.Mvc;
using Owin;
using Microsoft.Owin;

[assembly: OwinStartupAttribute( typeof( C8.TradeLicense.Startup ) )]
namespace C8.TradeLicense
{
    public partial class Startup
    {
        public void Configuration( IAppBuilder app )
        {
            ConfigureAuth( app );
        }
       
    }
}