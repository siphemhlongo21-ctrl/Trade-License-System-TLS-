using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C8.TradeLicense.Models.Interfaces
{
    public interface IUIMessage
    {
        string Message { get; set; }
        string MessageType { get; set; }
    }
}
