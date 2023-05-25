using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INOLAB_OC
{
    /// <summary>
    /// Descripción breve de Handler1
    /// </summary>
    public class Handler1 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "texto/normal";
            context.Response.Write("Hola a todos");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}