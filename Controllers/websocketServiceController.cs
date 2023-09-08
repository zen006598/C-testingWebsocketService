using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static serverapi.Models.websocketService;

namespace serverapi.Controllers
{
    [RoutePrefix("ws")]
    public class websocketServiceController : ApiController
    {   
        [HttpGet]
        [Route("NotifyBehavior")]
        public HttpResponseMessage Connect(string User)
        {
            if (NotifyBehavior.IsUserConnected(User))
            {
                return Request.CreateResponse("Connecting...");
            }
            else
            {
                return Request.CreateResponse("connecting error!");
            }
        }
    }
}