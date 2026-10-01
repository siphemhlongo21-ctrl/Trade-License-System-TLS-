using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using C8.Windows.Service;

namespace TradeLicenseWinService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            {
                new TradeLicenseService(), 
            };
            ServiceBase.Run(ServicesToRun);
        }

    }
}

