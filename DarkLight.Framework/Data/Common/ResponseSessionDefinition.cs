using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkLight.Framework.Data.Common
{
    public class ResponseSessionDefinition
    {
        public string ResponseType { get; set; }
        public string Parameters { get; set; }
        public DateTime TradeDate { get; set; }
        public string Products { get; set; }
    }
}
