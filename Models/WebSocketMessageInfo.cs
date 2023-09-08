using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace serverapi.Models
{
    public class WebSocketMessageInfo
    {
        public string Sender { get; set; }
        public string Reciver { get; set; }
        public string Content { get; set; }
    }
}